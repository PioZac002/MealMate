'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { ingredientsApi, recipesApi, type Ingredient, type RecipeInput } from '@/services/api';
import { useAuth } from '@/contexts/AuthContext';

const DIET_TYPES = ['Regular', 'Vegetarian', 'Vegan', 'GlutenFree', 'Keto', 'Other'];

type IngredientDraft = {
  ingredientId: string;
  quantity: string;
  unit: string;
};

type StepDraft = {
  description: string;
};

function createIngredientDraft(): IngredientDraft {
  return {
    ingredientId: '',
    quantity: '',
    unit: '',
  };
}

function createStepDraft(): StepDraft {
  return {
    description: '',
  };
}

export default function NewRecipePage() {
  const router = useRouter();
  const { isAuthenticated, isLoading } = useAuth();

  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [ingredientsLoading, setIngredientsLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  const [form, setForm] = useState({
    title: '',
    description: '',
    prepTimeMinutes: '10',
    cookTimeMinutes: '20',
    servings: '2',
    dietType: 'Regular',
    isPublic: true,
  });

  const [ingredientRows, setIngredientRows] = useState<IngredientDraft[]>([createIngredientDraft()]);
  const [stepRows, setStepRows] = useState<StepDraft[]>([createStepDraft()]);

  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      router.push('/login');
    }
  }, [isAuthenticated, isLoading, router]);

  useEffect(() => {
    if (!isAuthenticated) {
      return;
    }

    ingredientsApi.getAll({ pageSize: 200 })
      .then((response) => {
        if (response.data) {
          setIngredients(response.data.items);
          return;
        }

        setError(response.error || 'Failed to load ingredients.');
      })
      .finally(() => setIngredientsLoading(false));
  }, [isAuthenticated]);

  const handleIngredientChange = (
    index: number,
    field: keyof IngredientDraft,
    value: string
  ) => {
    setIngredientRows((current) => current.map((row, rowIndex) => {
      if (rowIndex !== index) {
        return row;
      }

      if (field !== 'ingredientId') {
        return { ...row, [field]: value };
      }

      const selectedIngredient = ingredients.find((ingredient) => ingredient.id === value);

      return {
        ...row,
        ingredientId: value,
        unit: selectedIngredient?.defaultUnit || row.unit,
      };
    }));
  };

  const handleStepChange = (index: number, description: string) => {
    setStepRows((current) => current.map((step, stepIndex) => (
      stepIndex === index ? { ...step, description } : step
    )));
  };

  const addIngredientRow = () => {
    setIngredientRows((current) => [...current, createIngredientDraft()]);
  };

  const removeIngredientRow = (index: number) => {
    setIngredientRows((current) => (
      current.length === 1 ? current : current.filter((_, rowIndex) => rowIndex !== index)
    ));
  };

  const addStepRow = () => {
    setStepRows((current) => [...current, createStepDraft()]);
  };

  const removeStepRow = (index: number) => {
    setStepRows((current) => (
      current.length === 1 ? current : current.filter((_, stepIndex) => stepIndex !== index)
    ));
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError('');

    const validIngredients = ingredientRows.filter((row) => row.ingredientId && row.quantity && row.unit);
    if (validIngredients.length === 0) {
      setError('Add at least one ingredient before saving.');
      return;
    }

    const validSteps = stepRows
      .map((step) => step.description.trim())
      .filter(Boolean);

    const payload: RecipeInput = {
      title: form.title.trim(),
      description: form.description.trim() || undefined,
      prepTimeMinutes: Number(form.prepTimeMinutes),
      cookTimeMinutes: Number(form.cookTimeMinutes),
      servings: Number(form.servings),
      dietType: form.dietType,
      isPublic: form.isPublic,
      ingredients: validIngredients.map((row) => ({
        ingredientId: row.ingredientId,
        quantity: Number(row.quantity),
        unit: row.unit.trim(),
      })),
      steps: validSteps.map((description, index) => ({
        stepNumber: index + 1,
        description,
      })),
    };

    setSubmitting(true);
    const response = await recipesApi.create(payload);
    setSubmitting(false);

    if (response.error || !response.data) {
      setError(response.error || 'Failed to create recipe.');
      return;
    }

    router.push(`/recipes/${response.data.id}`);
  };

  if (isLoading || ingredientsLoading) {
    return <div className="min-h-screen flex items-center justify-center">Loading...</div>;
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm border-b">
        <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex justify-between items-center">
          <Link href="/recipes" className="flex items-center gap-2">
            <span className="text-2xl">🍽️</span>
            <span className="text-xl font-bold text-green-700">MealMate</span>
          </Link>
          <div className="flex items-center gap-4 text-sm">
            <Link href="/dashboard" className="text-gray-600 hover:text-green-700">Dashboard</Link>
            <Link href="/recipes" className="text-green-700 font-semibold">Recipes</Link>
          </div>
        </div>
      </nav>

      <main className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="flex items-start justify-between gap-4 mb-8">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Create Recipe</h1>
            <p className="text-gray-500 mt-1">Add ingredients, steps, and nutrition-ready details.</p>
          </div>
          <Link
            href="/recipes"
            className="border border-gray-300 bg-white px-4 py-2 rounded-lg text-sm font-medium text-gray-700 hover:bg-gray-50"
          >
            Cancel
          </Link>
        </div>

        {error && (
          <div className="mb-6 rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-6">
          <section className="bg-white rounded-2xl border border-gray-100 shadow-sm p-6 space-y-4">
            <h2 className="text-lg font-semibold text-gray-900">Basics</h2>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">Title</label>
                <input
                  type="text"
                  value={form.title}
                  onChange={(event) => setForm((current) => ({ ...current, title: event.target.value }))}
                  className="w-full rounded-lg border border-gray-300 px-4 py-2.5 focus:outline-none focus:ring-2 focus:ring-green-500"
                  placeholder="e.g. Creamy chicken pasta"
                  required
                />
              </div>

              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">Description</label>
                <textarea
                  value={form.description}
                  onChange={(event) => setForm((current) => ({ ...current, description: event.target.value }))}
                  rows={3}
                  className="w-full rounded-lg border border-gray-300 px-4 py-2.5 focus:outline-none focus:ring-2 focus:ring-green-500"
                  placeholder="Short description of the recipe"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Prep Time (min)</label>
                <input
                  type="number"
                  min="0"
                  value={form.prepTimeMinutes}
                  onChange={(event) => setForm((current) => ({ ...current, prepTimeMinutes: event.target.value }))}
                  className="w-full rounded-lg border border-gray-300 px-4 py-2.5 focus:outline-none focus:ring-2 focus:ring-green-500"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Cook Time (min)</label>
                <input
                  type="number"
                  min="0"
                  value={form.cookTimeMinutes}
                  onChange={(event) => setForm((current) => ({ ...current, cookTimeMinutes: event.target.value }))}
                  className="w-full rounded-lg border border-gray-300 px-4 py-2.5 focus:outline-none focus:ring-2 focus:ring-green-500"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Servings</label>
                <input
                  type="number"
                  min="1"
                  value={form.servings}
                  onChange={(event) => setForm((current) => ({ ...current, servings: event.target.value }))}
                  className="w-full rounded-lg border border-gray-300 px-4 py-2.5 focus:outline-none focus:ring-2 focus:ring-green-500"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Diet Type</label>
                <select
                  value={form.dietType}
                  onChange={(event) => setForm((current) => ({ ...current, dietType: event.target.value }))}
                  className="w-full rounded-lg border border-gray-300 px-4 py-2.5 focus:outline-none focus:ring-2 focus:ring-green-500"
                >
                  {DIET_TYPES.map((dietType) => (
                    <option key={dietType} value={dietType}>
                      {dietType}
                    </option>
                  ))}
                </select>
              </div>
            </div>

            <label className="inline-flex items-center gap-3 text-sm text-gray-700">
              <input
                type="checkbox"
                checked={form.isPublic}
                onChange={(event) => setForm((current) => ({ ...current, isPublic: event.target.checked }))}
                className="h-4 w-4 rounded border-gray-300 text-green-600 focus:ring-green-500"
              />
              Make this recipe visible to other users
            </label>
          </section>

          <section className="bg-white rounded-2xl border border-gray-100 shadow-sm p-6 space-y-4">
            <div className="flex items-center justify-between gap-4">
              <div>
                <h2 className="text-lg font-semibold text-gray-900">Ingredients</h2>
                <p className="text-sm text-gray-500">Choose at least one ingredient for nutrition calculation.</p>
              </div>
              <button
                type="button"
                onClick={addIngredientRow}
                className="rounded-lg border border-green-200 bg-green-50 px-4 py-2 text-sm font-medium text-green-700 hover:bg-green-100"
              >
                + Add Ingredient
              </button>
            </div>

            <div className="space-y-3">
              {ingredientRows.map((row, index) => (
                <div key={index} className="grid grid-cols-1 md:grid-cols-[minmax(0,2fr)_140px_140px_auto] gap-3 items-end">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Ingredient {index + 1}
                    </label>
                    <select
                      value={row.ingredientId}
                      onChange={(event) => handleIngredientChange(index, 'ingredientId', event.target.value)}
                      className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
                      required={index === 0}
                    >
                      <option value="">Select ingredient...</option>
                      {ingredients.map((ingredient) => (
                        <option key={ingredient.id} value={ingredient.id}>
                          {ingredient.name}
                        </option>
                      ))}
                    </select>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Quantity</label>
                    <input
                      type="number"
                      min="0.01"
                      step="0.01"
                      value={row.quantity}
                      onChange={(event) => handleIngredientChange(index, 'quantity', event.target.value)}
                      className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
                      required={index === 0}
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Unit</label>
                    <input
                      type="text"
                      value={row.unit}
                      onChange={(event) => handleIngredientChange(index, 'unit', event.target.value)}
                      className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
                      placeholder="g, ml, pcs"
                      required={index === 0}
                    />
                  </div>

                  <button
                    type="button"
                    onClick={() => removeIngredientRow(index)}
                    disabled={ingredientRows.length === 1}
                    className="rounded-lg border border-gray-300 px-3 py-2.5 text-sm text-gray-600 hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-50"
                  >
                    Remove
                  </button>
                </div>
              ))}
            </div>
          </section>

          <section className="bg-white rounded-2xl border border-gray-100 shadow-sm p-6 space-y-4">
            <div className="flex items-center justify-between gap-4">
              <div>
                <h2 className="text-lg font-semibold text-gray-900">Steps</h2>
                <p className="text-sm text-gray-500">Instructions are optional, but keep the recipe easy to follow.</p>
              </div>
              <button
                type="button"
                onClick={addStepRow}
                className="rounded-lg border border-green-200 bg-green-50 px-4 py-2 text-sm font-medium text-green-700 hover:bg-green-100"
              >
                + Add Step
              </button>
            </div>

            <div className="space-y-3">
              {stepRows.map((step, index) => (
                <div key={index} className="flex gap-3 items-start">
                  <div className="mt-2 flex h-8 w-8 flex-shrink-0 items-center justify-center rounded-full bg-green-600 text-sm font-semibold text-white">
                    {index + 1}
                  </div>

                  <textarea
                    value={step.description}
                    onChange={(event) => handleStepChange(index, event.target.value)}
                    rows={3}
                    className="flex-1 rounded-lg border border-gray-300 px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
                    placeholder="Describe this step"
                  />

                  <button
                    type="button"
                    onClick={() => removeStepRow(index)}
                    disabled={stepRows.length === 1}
                    className="rounded-lg border border-gray-300 px-3 py-2.5 text-sm text-gray-600 hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-50"
                  >
                    Remove
                  </button>
                </div>
              ))}
            </div>
          </section>

          <div className="flex justify-end gap-3">
            <Link
              href="/recipes"
              className="rounded-lg border border-gray-300 bg-white px-5 py-2.5 text-sm font-medium text-gray-700 hover:bg-gray-50"
            >
              Cancel
            </Link>
            <button
              type="submit"
              disabled={submitting}
              className="rounded-lg bg-green-600 px-5 py-2.5 text-sm font-semibold text-white hover:bg-green-700 disabled:cursor-not-allowed disabled:opacity-50"
            >
              {submitting ? 'Saving...' : 'Create Recipe'}
            </button>
          </div>
        </form>
      </main>
    </div>
  );
}
