'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import Link from 'next/link';
import { recipesApi, RecipeDetail } from '@/services/api';
import { useAuth } from '@/contexts/AuthContext';

export default function RecipeDetailPage() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();
  const { user } = useAuth();
  const [recipe, setRecipe] = useState<RecipeDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;
    recipesApi.getById(id)
      .then(r => {
        if (r.data) setRecipe(r.data);
        else setError(r.error || 'Recipe not found');
      })
      .finally(() => setLoading(false));
  }, [id]);

  const handleDelete = async () => {
    if (!confirm('Are you sure you want to delete this recipe?')) return;
    await recipesApi.delete(id);
    router.push('/recipes');
  };

  if (loading) return <div className="min-h-screen flex items-center justify-center">Loading...</div>;
  if (error || !recipe) return (
    <div className="min-h-screen flex items-center justify-center text-red-600">{error || 'Not found'}</div>
  );

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm border-b">
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex justify-between items-center">
          <Link href="/recipes" className="text-green-700 hover:underline">Back to Recipes</Link>
          {user?.id === recipe.createdByUserId && (
            <button onClick={handleDelete} className="text-red-500 hover:text-red-700 text-sm">
              Delete Recipe
            </button>
          )}
        </div>
      </nav>

      <main className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div className="bg-gradient-to-br from-green-100 to-emerald-50 h-48 flex items-center justify-center">
            <span className="text-8xl">🍳</span>
          </div>
          <div className="p-8">
            <div className="flex justify-between items-start">
              <div>
                <h1 className="text-3xl font-bold text-gray-900">{recipe.title}</h1>
                <p className="text-gray-500 mt-2">{recipe.description}</p>
              </div>
              <span className="bg-green-100 text-green-700 px-3 py-1 rounded-full text-sm font-medium">
                {recipe.dietType}
              </span>
            </div>

            <div className="flex gap-6 mt-6 p-4 bg-gray-50 rounded-xl">
              <div className="text-center">
                <div className="text-2xl font-bold text-green-600">{recipe.prepTimeMinutes}</div>
                <div className="text-xs text-gray-500">Prep (min)</div>
              </div>
              <div className="text-center">
                <div className="text-2xl font-bold text-green-600">{recipe.cookTimeMinutes}</div>
                <div className="text-xs text-gray-500">Cook (min)</div>
              </div>
              <div className="text-center">
                <div className="text-2xl font-bold text-green-600">{recipe.servings}</div>
                <div className="text-xs text-gray-500">Servings</div>
              </div>
              <div className="text-center">
                <div className="text-2xl font-bold text-orange-500">{recipe.totalCalories}</div>
                <div className="text-xs text-gray-500">Cal (total)</div>
              </div>
            </div>

            <div className="mt-4 grid grid-cols-3 gap-4">
              <div className="bg-blue-50 p-3 rounded-lg text-center">
                <div className="font-semibold text-blue-700">{recipe.totalProtein}g</div>
                <div className="text-xs text-blue-500">Protein</div>
              </div>
              <div className="bg-yellow-50 p-3 rounded-lg text-center">
                <div className="font-semibold text-yellow-700">{recipe.totalCarbs}g</div>
                <div className="text-xs text-yellow-500">Carbs</div>
              </div>
              <div className="bg-red-50 p-3 rounded-lg text-center">
                <div className="font-semibold text-red-700">{recipe.totalFat}g</div>
                <div className="text-xs text-red-500">Fat</div>
              </div>
            </div>

            {recipe.ingredients.length > 0 && (
              <div className="mt-8">
                <h2 className="text-xl font-semibold text-gray-800 mb-4">Ingredients</h2>
                <ul className="space-y-2">
                  {recipe.ingredients.map(ing => (
                    <li key={ing.id} className="flex justify-between p-3 bg-gray-50 rounded-lg">
                      <span className="text-gray-700">{ing.ingredientName}</span>
                      <span className="text-gray-500">{ing.quantity} {ing.unit}</span>
                    </li>
                  ))}
                </ul>
              </div>
            )}

            {recipe.steps.length > 0 && (
              <div className="mt-8">
                <h2 className="text-xl font-semibold text-gray-800 mb-4">Instructions</h2>
                <ol className="space-y-4">
                  {recipe.steps
                    .sort((a, b) => a.stepNumber - b.stepNumber)
                    .map(step => (
                      <li key={step.id} className="flex gap-4">
                        <div className="w-8 h-8 bg-green-600 text-white rounded-full flex items-center justify-center font-bold text-sm flex-shrink-0 mt-1">
                          {step.stepNumber}
                        </div>
                        <p className="text-gray-700 leading-relaxed">{step.description}</p>
                      </li>
                    ))}
                </ol>
              </div>
            )}

            <p className="text-xs text-gray-400 mt-8">By {recipe.createdByUserName}</p>
          </div>
        </div>
      </main>
    </div>
  );
}
