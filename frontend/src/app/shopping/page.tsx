'use client';

import { useCallback, useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { useAuth } from '@/contexts/AuthContext';
import {
  shoppingApi, householdsApi, ingredientsApi,
  type ShoppingList, type ShoppingListDetail, type Household, type Ingredient,
} from '@/services/api';

export default function ShoppingPage() {
  const { isAuthenticated, isLoading, logout } = useAuth();
  const router = useRouter();

  const [households, setHouseholds] = useState<Household[]>([]);
  const [selectedHousehold, setSelectedHousehold] = useState('');
  const [lists, setLists] = useState<ShoppingList[]>([]);
  const [activeList, setActiveList] = useState<ShoppingListDetail | null>(null);
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const [showCreateList, setShowCreateList] = useState(false);
  const [newListName, setNewListName] = useState('');
  const [showAddItem, setShowAddItem] = useState(false);
  const [itemForm, setItemForm] = useState({ ingredientId: '', quantity: '', unit: '' });

  useEffect(() => {
    if (!isLoading && !isAuthenticated) router.push('/login');
  }, [isLoading, isAuthenticated, router]);

  useEffect(() => {
    if (isAuthenticated) {
      householdsApi.getMyHouseholds().then(r => {
        if (r.data && r.data.length > 0) {
          setHouseholds(r.data);
          setLoading(true);
          setActiveList(null);
          setSelectedHousehold(r.data[0].id);
        }
      });
      ingredientsApi.getAll({ pageSize: 200 }).then(r => {
        if (r.data) setIngredients(r.data.items);
      });
    }
  }, [isAuthenticated]);

  const loadLists = useCallback(async (householdId: string) => {
    setLoading(true);
    setActiveList(null);
    const r = await shoppingApi.getAll(householdId);
    if (r.data) setLists(r.data);
    setLoading(false);
  }, []);

  const refreshActiveList = useCallback(async () => {
    if (!activeList) return;
    const r = await shoppingApi.getById(selectedHousehold, activeList.id);
    const detail = r.data;
    if (detail) {
      setActiveList(detail);
      setLists(prev => prev.map(l => l.id === detail.id
        ? { ...l, itemCount: detail.items.length, boughtCount: detail.items.filter(i => i.isBought).length }
        : l
      ));
    }
  }, [activeList, selectedHousehold]);

  useEffect(() => {
    if (selectedHousehold) {
      shoppingApi.getAll(selectedHousehold)
        .then(r => {
          if (r.data) setLists(r.data);
        })
        .finally(() => setLoading(false));
    }
  }, [selectedHousehold]);

  const openList = async (listId: string) => {
    const r = await shoppingApi.getById(selectedHousehold, listId);
    if (r.data) setActiveList(r.data);
  };

  const handleCreateList = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    const r = await shoppingApi.create(selectedHousehold, { name: newListName });
    if (r.error) { setError(r.error); return; }
    setShowCreateList(false);
    setNewListName('');
    await loadLists(selectedHousehold);
    if (r.data) setActiveList(r.data);
  };

  const handleAddItem = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!activeList) return;
    setError('');
    const r = await shoppingApi.addItem(selectedHousehold, activeList.id, {
      ingredientId: itemForm.ingredientId,
      quantity: parseFloat(itemForm.quantity),
      unit: itemForm.unit,
    });
    if (r.error) { setError(r.error); return; }
    setShowAddItem(false);
    setItemForm({ ingredientId: '', quantity: '', unit: '' });
    await refreshActiveList();
  };

  const handleToggle = async (itemId: string) => {
    if (!activeList) return;
    await shoppingApi.toggleItem(selectedHousehold, activeList.id, itemId);
    await refreshActiveList();
  };

  const handleRemoveItem = async (itemId: string) => {
    if (!activeList) return;
    await shoppingApi.removeItem(selectedHousehold, activeList.id, itemId);
    await refreshActiveList();
  };

  const handleCompleteList = async () => {
    if (!activeList || !confirm('Mark this list as completed?')) return;
    const r = await shoppingApi.complete(selectedHousehold, activeList.id);
    if (r.data) {
      setActiveList(r.data);
      await loadLists(selectedHousehold);
    }
  };

  const handleDeleteList = async (listId: string) => {
    if (!confirm('Delete this shopping list?')) return;
    await shoppingApi.delete(selectedHousehold, listId);
    if (activeList?.id === listId) setActiveList(null);
    await loadLists(selectedHousehold);
  };

  if (isLoading) return <div className="min-h-screen flex items-center justify-center">Loading...</div>;

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex justify-between items-center">
          <Link href="/dashboard" className="flex items-center gap-2">
            <span className="text-2xl">🍽️</span>
            <span className="text-xl font-bold text-green-700">MealMate</span>
          </Link>
          <div className="flex items-center gap-6">
            <Link href="/recipes" className="text-gray-600 hover:text-green-700">Recipes</Link>
            <Link href="/households" className="text-gray-600 hover:text-green-700">Household</Link>
            <Link href="/fridge" className="text-gray-600 hover:text-green-700">Fridge</Link>
            <Link href="/shopping" className="text-green-700 font-semibold">Shopping</Link>
            <button onClick={() => { logout(); router.push('/'); }} className="text-gray-500 hover:text-red-600 text-sm">Sign Out</button>
          </div>
        </div>
      </nav>

      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="flex justify-between items-center mb-6">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">🛒 Shopping Lists</h1>
            <p className="text-gray-500 mt-1">Manage your household shopping</p>
          </div>
          <div className="flex items-center gap-3">
            {households.length > 1 && (
              <select
                value={selectedHousehold}
                onChange={e => {
                  setLoading(true);
                  setActiveList(null);
                  setSelectedHousehold(e.target.value);
                }}
                className="border border-gray-300 rounded-lg px-3 py-2 text-sm"
              >
                {households.map(h => <option key={h.id} value={h.id}>{h.name}</option>)}
              </select>
            )}
            <button
              onClick={() => setShowCreateList(true)}
              className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors text-sm font-medium"
            >
              + New List
            </button>
          </div>
        </div>

        {error && <div className="bg-red-50 text-red-700 px-4 py-3 rounded-lg mb-4">{error}</div>}

        {showCreateList && (
          <div className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100 mb-6">
            <h2 className="text-lg font-semibold mb-4">Create Shopping List</h2>
            <form onSubmit={handleCreateList} className="flex gap-3">
              <input
                type="text" required placeholder="List name (e.g. Weekly groceries)"
                value={newListName}
                onChange={e => setNewListName(e.target.value)}
                className="flex-1 border border-gray-300 rounded-lg px-3 py-2 text-sm"
              />
              <button type="submit" className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 text-sm font-medium">Create</button>
              <button type="button" onClick={() => setShowCreateList(false)} className="px-4 py-2 rounded-lg border border-gray-300 text-sm hover:bg-gray-50">Cancel</button>
            </form>
          </div>
        )}

        {households.length === 0 ? (
          <div className="text-center py-12">
            <p className="text-gray-500 mb-4">You need to be in a household to use shopping lists.</p>
            <Link href="/households" className="text-green-600 hover:underline">Go to Households →</Link>
          </div>
        ) : (
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
            {/* Lists panel */}
            <div className="lg:col-span-1">
              <h2 className="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-3">Your Lists</h2>
              {loading ? (
                <p className="text-gray-400 text-sm">Loading…</p>
              ) : lists.length === 0 ? (
                <div className="bg-white rounded-2xl p-6 border border-gray-100 text-center">
                  <p className="text-gray-400 text-sm">No lists yet. Create one!</p>
                </div>
              ) : (
                <div className="space-y-2">
                  {lists.map(list => (
                    <div
                      key={list.id}
                      className={`bg-white rounded-xl p-4 border cursor-pointer transition-all ${activeList?.id === list.id ? 'border-green-400 shadow-md' : 'border-gray-100 hover:shadow-sm'}`}
                      onClick={() => openList(list.id)}
                    >
                      <div className="flex justify-between items-start">
                        <div className="flex-1 min-w-0">
                          <p className={`font-semibold truncate ${list.isCompleted ? 'text-gray-400 line-through' : 'text-gray-800'}`}>
                            {list.name}
                          </p>
                          <p className="text-xs text-gray-400 mt-0.5">
                            {list.boughtCount}/{list.itemCount} items bought
                          </p>
                        </div>
                        <button
                          onClick={e => { e.stopPropagation(); handleDeleteList(list.id); }}
                          className="text-gray-300 hover:text-red-500 ml-2 text-sm"
                        >🗑️</button>
                      </div>
                      {list.itemCount > 0 && (
                        <div className="mt-2 bg-gray-100 rounded-full h-1.5 overflow-hidden">
                          <div
                            className="bg-green-500 h-full rounded-full transition-all"
                            style={{ width: `${(list.boughtCount / list.itemCount) * 100}%` }}
                          />
                        </div>
                      )}
                      {list.isCompleted && (
                        <p className="text-xs text-green-600 mt-1 font-medium">✓ Completed</p>
                      )}
                    </div>
                  ))}
                </div>
              )}
            </div>

            {/* Active list detail */}
            <div className="lg:col-span-2">
              {activeList ? (
                <div className="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
                  <div className="p-6 border-b border-gray-100">
                    <div className="flex justify-between items-center">
                      <div>
                        <h2 className="text-xl font-bold text-gray-900">{activeList.name}</h2>
                        <p className="text-sm text-gray-400 mt-0.5">
                          {activeList.items.filter(i => i.isBought).length} of {activeList.items.length} items bought
                        </p>
                      </div>
                      <div className="flex gap-2">
                        {!activeList.isCompleted && (
                          <>
                            <button
                              onClick={() => setShowAddItem(!showAddItem)}
                              className="bg-green-600 text-white px-3 py-1.5 rounded-lg hover:bg-green-700 text-sm font-medium"
                            >
                              + Add Item
                            </button>
                            {activeList.items.length > 0 && (
                              <button
                                onClick={handleCompleteList}
                                className="border border-green-500 text-green-600 px-3 py-1.5 rounded-lg hover:bg-green-50 text-sm font-medium"
                              >
                                Complete
                              </button>
                            )}
                          </>
                        )}
                      </div>
                    </div>

                    {showAddItem && !activeList.isCompleted && (
                      <form onSubmit={handleAddItem} className="mt-4 grid grid-cols-1 sm:grid-cols-3 gap-3">
                        <select
                          required
                          value={itemForm.ingredientId}
                          onChange={e => {
                            const ing = ingredients.find(i => i.id === e.target.value);
                            setItemForm(f => ({ ...f, ingredientId: e.target.value, unit: ing?.defaultUnit || f.unit }));
                          }}
                          className="border border-gray-300 rounded-lg px-3 py-2 text-sm"
                        >
                          <option value="">Select ingredient…</option>
                          {ingredients.map(i => <option key={i.id} value={i.id}>{i.name}</option>)}
                        </select>
                        <input
                          type="number" required min="0.01" step="0.01"
                          placeholder="Quantity"
                          value={itemForm.quantity}
                          onChange={e => setItemForm(f => ({ ...f, quantity: e.target.value }))}
                          className="border border-gray-300 rounded-lg px-3 py-2 text-sm"
                        />
                        <div className="flex gap-2">
                          <input
                            type="text" required placeholder="Unit"
                            value={itemForm.unit}
                            onChange={e => setItemForm(f => ({ ...f, unit: e.target.value }))}
                            className="flex-1 border border-gray-300 rounded-lg px-3 py-2 text-sm"
                          />
                          <button type="submit" className="bg-green-600 text-white px-3 py-2 rounded-lg hover:bg-green-700 text-sm">Add</button>
                          <button type="button" onClick={() => setShowAddItem(false)} className="px-3 py-2 rounded-lg border border-gray-300 text-sm">✕</button>
                        </div>
                      </form>
                    )}
                  </div>

                  <div className="divide-y divide-gray-50">
                    {activeList.items.length === 0 ? (
                      <div className="p-8 text-center text-gray-400">No items yet. Add some items!</div>
                    ) : (
                      activeList.items.map(item => (
                        <div key={item.id} className={`flex items-center gap-4 px-6 py-3 hover:bg-gray-50 transition-colors ${item.isBought ? 'opacity-60' : ''}`}>
                          <button
                            onClick={() => handleToggle(item.id)}
                            className={`w-5 h-5 rounded-full border-2 flex-shrink-0 flex items-center justify-center transition-colors ${item.isBought ? 'bg-green-500 border-green-500' : 'border-gray-300 hover:border-green-400'}`}
                          >
                            {item.isBought && <span className="text-white text-xs">✓</span>}
                          </button>
                          <div className="flex-1 min-w-0">
                            <span className={`font-medium ${item.isBought ? 'line-through text-gray-400' : 'text-gray-800'}`}>
                              {item.ingredientName}
                            </span>
                            <span className="text-gray-500 text-sm ml-2">{item.quantity} {item.unit}</span>
                          </div>
                          {!activeList.isCompleted && (
                            <button
                              onClick={() => handleRemoveItem(item.id)}
                              className="text-gray-300 hover:text-red-500 text-sm flex-shrink-0"
                            >🗑️</button>
                          )}
                        </div>
                      ))
                    )}
                  </div>
                </div>
              ) : (
                <div className="bg-white rounded-2xl p-12 shadow-sm border border-gray-100 text-center">
                  <div className="text-5xl mb-3">🛒</div>
                  <p className="text-gray-400">Select a list to view its items</p>
                </div>
              )}
            </div>
          </div>
        )}
      </main>
    </div>
  );
}
