'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { useAuth } from '@/contexts/AuthContext';
import { fridgeApi, householdsApi, ingredientsApi, type FridgeItem, type Household, type Ingredient } from '@/services/api';

export default function FridgePage() {
  const { isAuthenticated, isLoading, logout } = useAuth();
  const router = useRouter();

  const [households, setHouseholds] = useState<Household[]>([]);
  const [selectedHousehold, setSelectedHousehold] = useState<string>('');
  const [fridgeItems, setFridgeItems] = useState<FridgeItem[]>([]);
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [showAddForm, setShowAddForm] = useState(false);
  const [editItem, setEditItem] = useState<FridgeItem | null>(null);

  const [form, setForm] = useState({
    ingredientId: '',
    quantity: '',
    unit: '',
    expiryDate: '',
  });

  useEffect(() => {
    if (!isLoading && !isAuthenticated) router.push('/login');
  }, [isLoading, isAuthenticated, router]);

  useEffect(() => {
    if (isAuthenticated) {
      householdsApi.getMyHouseholds().then(r => {
        if (r.data && r.data.length > 0) {
          setHouseholds(r.data);
          setSelectedHousehold(r.data[0].id);
        }
      });
      ingredientsApi.getAll({ pageSize: 200 }).then(r => {
        if (r.data) setIngredients(r.data.items);
      });
    }
  }, [isAuthenticated]);

  useEffect(() => {
    if (selectedHousehold) loadFridge();
  }, [selectedHousehold]);

  const loadFridge = async () => {
    if (!selectedHousehold) return;
    setLoading(true);
    const r = await fridgeApi.getAll(selectedHousehold);
    if (r.data) setFridgeItems(r.data);
    setLoading(false);
  };

  const handleAdd = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    const r = await fridgeApi.add(selectedHousehold, {
      ingredientId: form.ingredientId,
      quantity: parseFloat(form.quantity),
      unit: form.unit,
      expiryDate: form.expiryDate || undefined,
    });
    if (r.error) { setError(r.error); return; }
    setShowAddForm(false);
    setForm({ ingredientId: '', quantity: '', unit: '', expiryDate: '' });
    loadFridge();
  };

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editItem) return;
    setError('');
    const r = await fridgeApi.update(selectedHousehold, editItem.id, {
      quantity: parseFloat(form.quantity),
      unit: form.unit,
      expiryDate: form.expiryDate || undefined,
    });
    if (r.error) { setError(r.error); return; }
    setEditItem(null);
    loadFridge();
  };

  const handleDelete = async (itemId: string) => {
    if (!confirm('Remove this item from the fridge?')) return;
    await fridgeApi.delete(selectedHousehold, itemId);
    loadFridge();
  };

  const openEdit = (item: FridgeItem) => {
    setEditItem(item);
    setForm({
      ingredientId: item.ingredientId,
      quantity: String(item.quantity),
      unit: item.unit,
      expiryDate: item.expiryDate ? item.expiryDate.split('T')[0] : '',
    });
    setShowAddForm(false);
  };

  const expiredItems = fridgeItems.filter(i => i.isExpired);
  const expiringSoon = fridgeItems.filter(i => !i.isExpired && i.isExpiringSoon);
  const normalItems = fridgeItems.filter(i => !i.isExpired && !i.isExpiringSoon);

  if (isLoading) return <div className="min-h-screen flex items-center justify-center">Loading...</div>;

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex justify-between items-center">
          <div className="flex items-center gap-4">
            <Link href="/dashboard" className="flex items-center gap-2">
              <span className="text-2xl">🍽️</span>
              <span className="text-xl font-bold text-green-700">MealMate</span>
            </Link>
          </div>
          <div className="flex items-center gap-6">
            <Link href="/recipes" className="text-gray-600 hover:text-green-700">Recipes</Link>
            <Link href="/households" className="text-gray-600 hover:text-green-700">Household</Link>
            <Link href="/fridge" className="text-green-700 font-semibold">Fridge</Link>
            <Link href="/shopping" className="text-gray-600 hover:text-green-700">Shopping</Link>
            <button onClick={() => { logout(); router.push('/'); }} className="text-gray-500 hover:text-red-600 text-sm">Sign Out</button>
          </div>
        </div>
      </nav>

      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="flex justify-between items-center mb-6">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">🧊 Smart Fridge</h1>
            <p className="text-gray-500 mt-1">Track what&apos;s in your fridge</p>
          </div>
          <div className="flex items-center gap-3">
            {households.length > 1 && (
              <select
                value={selectedHousehold}
                onChange={e => setSelectedHousehold(e.target.value)}
                className="border border-gray-300 rounded-lg px-3 py-2 text-sm"
              >
                {households.map(h => <option key={h.id} value={h.id}>{h.name}</option>)}
              </select>
            )}
            <button
              onClick={() => { setShowAddForm(!showAddForm); setEditItem(null); }}
              className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors text-sm font-medium"
            >
              + Add Item
            </button>
          </div>
        </div>

        {error && <div className="bg-red-50 text-red-700 px-4 py-3 rounded-lg mb-4">{error}</div>}

        {/* Add Form */}
        {showAddForm && (
          <div className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100 mb-6">
            <h2 className="text-lg font-semibold mb-4">Add Item to Fridge</h2>
            <form onSubmit={handleAdd} className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Ingredient</label>
                <select
                  required
                  value={form.ingredientId}
                  onChange={e => {
                    const ing = ingredients.find(i => i.id === e.target.value);
                    setForm(f => ({ ...f, ingredientId: e.target.value, unit: ing?.defaultUnit || f.unit }));
                  }}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm"
                >
                  <option value="">Select ingredient…</option>
                  {ingredients.map(i => <option key={i.id} value={i.id}>{i.name}</option>)}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Quantity</label>
                <input
                  type="number" required min="0.01" step="0.01"
                  value={form.quantity}
                  onChange={e => setForm(f => ({ ...f, quantity: e.target.value }))}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm"
                  placeholder="e.g. 500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Unit</label>
                <input
                  type="text" required
                  value={form.unit}
                  onChange={e => setForm(f => ({ ...f, unit: e.target.value }))}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm"
                  placeholder="e.g. g, ml, pcs"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Expiry Date</label>
                <input
                  type="date"
                  value={form.expiryDate}
                  onChange={e => setForm(f => ({ ...f, expiryDate: e.target.value }))}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm"
                />
              </div>
              <div className="sm:col-span-2 lg:col-span-4 flex gap-2">
                <button type="submit" className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 text-sm font-medium">Add to Fridge</button>
                <button type="button" onClick={() => setShowAddForm(false)} className="px-4 py-2 rounded-lg border border-gray-300 text-sm hover:bg-gray-50">Cancel</button>
              </div>
            </form>
          </div>
        )}

        {/* Edit Form */}
        {editItem && (
          <div className="bg-white rounded-2xl p-6 shadow-sm border border-blue-200 mb-6">
            <h2 className="text-lg font-semibold mb-4">Edit: {editItem.ingredientName}</h2>
            <form onSubmit={handleUpdate} className="grid grid-cols-1 sm:grid-cols-3 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Quantity</label>
                <input
                  type="number" required min="0.01" step="0.01"
                  value={form.quantity}
                  onChange={e => setForm(f => ({ ...f, quantity: e.target.value }))}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Unit</label>
                <input
                  type="text" required
                  value={form.unit}
                  onChange={e => setForm(f => ({ ...f, unit: e.target.value }))}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Expiry Date</label>
                <input
                  type="date"
                  value={form.expiryDate}
                  onChange={e => setForm(f => ({ ...f, expiryDate: e.target.value }))}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm"
                />
              </div>
              <div className="sm:col-span-3 flex gap-2">
                <button type="submit" className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 text-sm font-medium">Save Changes</button>
                <button type="button" onClick={() => setEditItem(null)} className="px-4 py-2 rounded-lg border border-gray-300 text-sm hover:bg-gray-50">Cancel</button>
              </div>
            </form>
          </div>
        )}

        {loading ? (
          <div className="text-center py-12 text-gray-500">Loading fridge…</div>
        ) : households.length === 0 ? (
          <div className="text-center py-12">
            <p className="text-gray-500 mb-4">You need to be in a household to use the fridge.</p>
            <Link href="/households" className="text-green-600 hover:underline">Go to Households →</Link>
          </div>
        ) : fridgeItems.length === 0 ? (
          <div className="text-center py-12 bg-white rounded-2xl border border-gray-100">
            <div className="text-5xl mb-3">🧊</div>
            <p className="text-gray-500">Your fridge is empty. Add your first item!</p>
          </div>
        ) : (
          <div className="space-y-6">
            {expiredItems.length > 0 && (
              <Section title="⚠️ Expired" color="red" items={expiredItems} onEdit={openEdit} onDelete={handleDelete} />
            )}
            {expiringSoon.length > 0 && (
              <Section title="⏰ Expiring Soon (within 3 days)" color="yellow" items={expiringSoon} onEdit={openEdit} onDelete={handleDelete} />
            )}
            {normalItems.length > 0 && (
              <Section title="✅ Fresh" color="green" items={normalItems} onEdit={openEdit} onDelete={handleDelete} />
            )}
          </div>
        )}
      </main>
    </div>
  );
}

function Section({
  title, color, items, onEdit, onDelete
}: {
  title: string;
  color: 'red' | 'yellow' | 'green';
  items: FridgeItem[];
  onEdit: (item: FridgeItem) => void;
  onDelete: (id: string) => void;
}) {
  const bg = { red: 'bg-red-50 border-red-100', yellow: 'bg-yellow-50 border-yellow-100', green: 'bg-white border-gray-100' }[color];
  return (
    <div>
      <h2 className="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-3">{title}</h2>
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
        {items.map(item => (
          <div key={item.id} className={`rounded-2xl p-4 border ${bg} flex flex-col gap-2`}>
            <div className="flex justify-between items-start">
              <div>
                <p className="font-semibold text-gray-800">{item.ingredientName}</p>
                <p className="text-xs text-gray-400">{item.ingredientCategory}</p>
              </div>
              <div className="flex gap-1">
                <button onClick={() => onEdit(item)} className="text-blue-500 hover:text-blue-700 text-xs p-1">✏️</button>
                <button onClick={() => onDelete(item.id)} className="text-red-400 hover:text-red-600 text-xs p-1">🗑️</button>
              </div>
            </div>
            <p className="text-lg font-bold text-gray-900">{item.quantity} {item.unit}</p>
            {item.expiryDate && (
              <p className={`text-xs ${item.isExpired ? 'text-red-600 font-semibold' : item.isExpiringSoon ? 'text-yellow-600 font-semibold' : 'text-gray-400'}`}>
                Expires: {new Date(item.expiryDate).toLocaleDateString()}
              </p>
            )}
            <p className="text-xs text-gray-400">Added by {item.addedByUserName}</p>
          </div>
        ))}
      </div>
    </div>
  );
}
