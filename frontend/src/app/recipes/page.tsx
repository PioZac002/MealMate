'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { recipesApi, PagedResult, Recipe } from '@/services/api';

const DIET_TYPES = ['Regular', 'Vegetarian', 'Vegan', 'GlutenFree', 'Keto', 'Other'];

export default function RecipesPage() {
  const [recipes, setRecipes] = useState<PagedResult<Recipe> | null>(null);
  const [search, setSearch] = useState('');
  const [dietType, setDietType] = useState('');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    setLoading(true);
    recipesApi.getAll({ search: search || undefined, dietType: dietType || undefined })
      .then(r => {
        if (r.data) setRecipes(r.data);
      })
      .finally(() => setLoading(false));
  }, [search, dietType]);

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex justify-between items-center">
          <Link href="/" className="flex items-center gap-2">
            <span className="text-2xl">🍽️</span>
            <span className="text-xl font-bold text-green-700">MealMate</span>
          </Link>
          <div className="flex items-center gap-4">
            <Link href="/dashboard" className="text-gray-600 hover:text-green-700 text-sm">Dashboard</Link>
            <Link href="/households" className="text-gray-600 hover:text-green-700 text-sm">Household</Link>
            <Link href="/fridge" className="text-gray-600 hover:text-green-700 text-sm">Fridge</Link>
            <Link href="/shopping" className="text-gray-600 hover:text-green-700 text-sm">Shopping</Link>
          </div>
        </div>
      </nav>

      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-3xl font-bold text-gray-900">Recipes</h1>
          <Link
            href="/recipes/new"
            className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors"
          >
            + New Recipe
          </Link>
        </div>

        <div className="flex gap-4 mb-6">
          <input
            type="text"
            placeholder="Search recipes..."
            value={search}
            onChange={e => setSearch(e.target.value)}
            className="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-green-500"
          />
          <select
            value={dietType}
            onChange={e => setDietType(e.target.value)}
            className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-green-500"
          >
            <option value="">All diets</option>
            {DIET_TYPES.map(d => <option key={d} value={d}>{d}</option>)}
          </select>
        </div>

        {loading ? (
          <div className="text-center py-20 text-gray-400">Loading recipes...</div>
        ) : recipes?.items.length === 0 ? (
          <div className="text-center py-20 text-gray-400">
            No recipes found.{' '}
            <Link href="/recipes/new" className="text-green-600 hover:underline">Create the first one!</Link>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {recipes?.items.map((recipe) => (
              <Link key={recipe.id} href={`/recipes/${recipe.id}`} className="bg-white rounded-2xl shadow-sm border border-gray-100 hover:shadow-md transition-shadow overflow-hidden block">
                <div className="bg-gradient-to-br from-green-100 to-emerald-50 h-40 flex items-center justify-center">
                  <span className="text-6xl">🍳</span>
                </div>
                <div className="p-5">
                  <div className="flex justify-between items-start mb-2">
                    <h3 className="text-lg font-semibold text-gray-800">{recipe.title}</h3>
                    <span className="text-xs bg-green-100 text-green-700 px-2 py-1 rounded-full">{recipe.dietType}</span>
                  </div>
                  <p className="text-gray-500 text-sm line-clamp-2">{recipe.description}</p>
                  <div className="flex gap-4 mt-3 text-xs text-gray-400">
                    <span>⏱️ {recipe.prepTimeMinutes + recipe.cookTimeMinutes} min</span>
                    <span>🍽️ {recipe.servings} servings</span>
                  </div>
                </div>
              </Link>
            ))}
          </div>
        )}

        {recipes && recipes.totalPages > 1 && (
          <div className="text-center mt-8 text-gray-500 text-sm">
            Showing {recipes.items.length} of {recipes.totalCount} recipes
          </div>
        )}
      </main>
    </div>
  );
}
