const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:8080/api';

export interface ApiResponse<T> {
  data?: T;
  error?: string;
}

async function request<T>(
  path: string,
  options: RequestInit = {}
): Promise<ApiResponse<T>> {
  const token = typeof window !== 'undefined' ? localStorage.getItem('accessToken') : null;

  const headers: HeadersInit = {
    'Content-Type': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
    ...(options.headers || {}),
  };

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers,
  });

  if (response.status === 204) {
    return { data: undefined };
  }

  const json = await response.json();

  if (!response.ok) {
    return { error: json.error || json.title || 'An error occurred' };
  }

  return { data: json };
}

export const api = {
  get: <T>(path: string) => request<T>(path, { method: 'GET' }),
  post: <T>(path: string, body: unknown) =>
    request<T>(path, { method: 'POST', body: JSON.stringify(body) }),
  put: <T>(path: string, body: unknown) =>
    request<T>(path, { method: 'PUT', body: JSON.stringify(body) }),
  patch: <T>(path: string, body: unknown) =>
    request<T>(path, { method: 'PATCH', body: JSON.stringify(body) }),
  delete: <T>(path: string) => request<T>(path, { method: 'DELETE' }),
};

// Auth
export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: User;
}

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  avatarUrl?: string;
  createdAt: string;
}

export const authApi = {
  register: (data: { email: string; password: string; firstName: string; lastName: string }) =>
    api.post<AuthResponse>('/auth/register', data),
  login: (data: { email: string; password: string }) =>
    api.post<AuthResponse>('/auth/login', data),
  refresh: (refreshToken: string) =>
    api.post<AuthResponse>('/auth/refresh', { refreshToken }),
  revoke: () => api.post('/auth/revoke', {}),
};

// Ingredients
export interface Ingredient {
  id: string;
  name: string;
  defaultUnit: string;
  category: string;
  caloriesPer100g: number;
  proteinPer100g: number;
  carbsPer100g: number;
  fatPer100g: number;
  imageUrl?: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export const ingredientsApi = {
  getAll: (params?: { search?: string; category?: string; page?: number; pageSize?: number }) => {
    const query = new URLSearchParams();
    if (params?.search) query.set('search', params.search);
    if (params?.category) query.set('category', params.category);
    if (params?.page) query.set('page', String(params.page));
    if (params?.pageSize) query.set('pageSize', String(params.pageSize));
    return api.get<PagedResult<Ingredient>>(`/ingredients?${query}`);
  },
  getById: (id: string) => api.get<Ingredient>(`/ingredients/${id}`),
  create: (data: Omit<Ingredient, 'id'>) => api.post<Ingredient>('/ingredients', data),
  update: (id: string, data: Omit<Ingredient, 'id'>) =>
    api.put<Ingredient>(`/ingredients/${id}`, data),
  delete: (id: string) => api.delete(`/ingredients/${id}`),
};

// Recipes
export interface Recipe {
  id: string;
  title: string;
  description?: string;
  prepTimeMinutes: number;
  cookTimeMinutes: number;
  servings: number;
  dietType: string;
  imageUrl?: string;
  isPublic: boolean;
  createdAt: string;
  createdByUserId: string;
  createdByUserName: string;
}

export interface RecipeDetail extends Recipe {
  ingredients: RecipeIngredient[];
  steps: RecipeStep[];
  totalCalories: number;
  totalProtein: number;
  totalCarbs: number;
  totalFat: number;
}

export interface RecipeIngredient {
  id: string;
  ingredientId: string;
  ingredientName: string;
  quantity: number;
  unit: string;
  caloriesPer100g: number;
}

export interface RecipeStep {
  id: string;
  stepNumber: number;
  description: string;
  imageUrl?: string;
}

export interface RecipeInputIngredient {
  ingredientId: string;
  quantity: number;
  unit: string;
}

export interface RecipeInputStep {
  stepNumber: number;
  description: string;
  imageUrl?: string;
}

export interface RecipeInput {
  title: string;
  description?: string;
  prepTimeMinutes: number;
  cookTimeMinutes: number;
  servings: number;
  dietType: string;
  imageUrl?: string;
  isPublic: boolean;
  ingredients: RecipeInputIngredient[];
  steps: RecipeInputStep[];
}

export const recipesApi = {
  getAll: (params?: { search?: string; dietType?: string; page?: number; pageSize?: number }) => {
    const query = new URLSearchParams();
    if (params?.search) query.set('search', params.search);
    if (params?.dietType) query.set('dietType', params.dietType);
    if (params?.page) query.set('page', String(params.page));
    if (params?.pageSize) query.set('pageSize', String(params.pageSize));
    return api.get<PagedResult<Recipe>>(`/recipes?${query}`);
  },
  getById: (id: string) => api.get<RecipeDetail>(`/recipes/${id}`),
  create: (data: RecipeInput) => api.post<RecipeDetail>('/recipes', data),
  update: (id: string, data: RecipeInput) => api.put<RecipeDetail>(`/recipes/${id}`, data),
  delete: (id: string) => api.delete(`/recipes/${id}`),
};

// Households
export interface Household {
  id: string;
  name: string;
  createdAt: string;
  createdByUserId: string;
  createdByUserName: string;
  memberCount: number;
}

export interface HouseholdDetail extends Household {
  members: HouseholdMember[];
}

export interface HouseholdMember {
  userId: string;
  email: string;
  fullName: string;
  avatarUrl?: string;
  role: string;
  joinedAt: string;
}

export const householdsApi = {
  getMyHouseholds: () => api.get<Household[]>('/households'),
  getById: (id: string) => api.get<HouseholdDetail>(`/households/${id}`),
  create: (data: { name: string }) => api.post<Household>('/households', data),
  update: (id: string, data: { name: string }) => api.put<Household>(`/households/${id}`, data),
  delete: (id: string) => api.delete(`/households/${id}`),
  invite: (id: string, email: string) => api.post(`/households/${id}/invite`, { email }),
  join: (code: string) => api.post<Household>('/households/join', { code }),
  removeMember: (householdId: string, memberId: string) =>
    api.delete(`/households/${householdId}/members/${memberId}`),
};

// Fridge
export interface FridgeItem {
  id: string;
  householdId: string;
  ingredientId: string;
  ingredientName: string;
  ingredientCategory: string;
  ingredientImageUrl?: string;
  quantity: number;
  unit: string;
  expiryDate?: string;
  addedAt: string;
  source: string;
  addedByUserName: string;
  isExpiringSoon: boolean;
  isExpired: boolean;
}

export const fridgeApi = {
  getAll: (householdId: string) =>
    api.get<FridgeItem[]>(`/households/${householdId}/fridge`),
  add: (householdId: string, data: { ingredientId: string; quantity: number; unit: string; expiryDate?: string }) =>
    api.post<FridgeItem>(`/households/${householdId}/fridge`, data),
  update: (householdId: string, itemId: string, data: { quantity: number; unit: string; expiryDate?: string }) =>
    api.put<FridgeItem>(`/households/${householdId}/fridge/${itemId}`, data),
  delete: (householdId: string, itemId: string) =>
    api.delete(`/households/${householdId}/fridge/${itemId}`),
};

// Shopping Lists
export interface ShoppingList {
  id: string;
  householdId: string;
  name: string;
  createdAt: string;
  isCompleted: boolean;
  completedAt?: string;
  itemCount: number;
  boughtCount: number;
}

export interface ShoppingListDetail extends Omit<ShoppingList, 'itemCount' | 'boughtCount'> {
  items: ShoppingListItem[];
}

export interface ShoppingListItem {
  id: string;
  shoppingListId: string;
  ingredientId: string;
  ingredientName: string;
  ingredientImageUrl?: string;
  quantity: number;
  unit: string;
  isBought: boolean;
  boughtAt?: string;
  source: string;
}

export const shoppingApi = {
  getAll: (householdId: string) =>
    api.get<ShoppingList[]>(`/households/${householdId}/shopping-lists`),
  getById: (householdId: string, listId: string) =>
    api.get<ShoppingListDetail>(`/households/${householdId}/shopping-lists/${listId}`),
  create: (householdId: string, data: { name: string }) =>
    api.post<ShoppingListDetail>(`/households/${householdId}/shopping-lists`, data),
  addItem: (householdId: string, listId: string, data: { ingredientId: string; quantity: number; unit: string }) =>
    api.post<ShoppingListItem>(`/households/${householdId}/shopping-lists/${listId}/items`, data),
  toggleItem: (householdId: string, listId: string, itemId: string) =>
    api.patch<ShoppingListItem>(`/households/${householdId}/shopping-lists/${listId}/items/${itemId}/toggle`, {}),
  removeItem: (householdId: string, listId: string, itemId: string) =>
    api.delete(`/households/${householdId}/shopping-lists/${listId}/items/${itemId}`),
  complete: (householdId: string, listId: string) =>
    api.post<ShoppingListDetail>(`/households/${householdId}/shopping-lists/${listId}/complete`, {}),
  delete: (householdId: string, listId: string) =>
    api.delete(`/households/${householdId}/shopping-lists/${listId}`),
};

// Nutrition
export interface DailyNutritionLog {
  id: string;
  date: string;
  calorieGoal: number;
  proteinGoal: number;
  carbsGoal: number;
  fatGoal: number;
  notes?: string;
  mealLogs: MealLog[];
  totalCalories: number;
  totalProtein: number;
  totalCarbs: number;
  totalFat: number;
}

export interface MealLog {
  id: string;
  mealType: string;
  recipeId?: string;
  recipeName?: string;
  customFoodName?: string;
  calories: number;
  protein: number;
  carbs: number;
  fat: number;
  servings: number;
  loggedAt: string;
  foodName: string;
}

export const nutritionApi = {
  getToday: () => api.get<DailyNutritionLog>('/nutrition/today'),
  getByDate: (date: string) => api.get<DailyNutritionLog>(`/nutrition/${date}`),
  setGoals: (date: string, data: { calorieGoal: number; proteinGoal: number; carbsGoal: number; fatGoal: number; notes?: string }) =>
    api.put<DailyNutritionLog>(`/nutrition/${date}/goals`, data),
  addMeal: (date: string, data: { mealType: string; recipeId?: string; customFoodName?: string; calories: number; protein: number; carbs: number; fat: number; servings: number }) =>
    api.post<MealLog>(`/nutrition/${date}/meals`, data),
  removeMeal: (mealLogId: string) => api.delete(`/nutrition/meals/${mealLogId}`),
  getHistory: (days?: number) => api.get<DailyNutritionLog[]>(`/nutrition/history${days ? `?days=${days}` : ''}`),
};

// Fitness
export interface Exercise {
  id: string;
  name: string;
  muscleGroup: string;
  description?: string;
  caloriesPerMinute: number;
  imageUrl?: string;
}

export interface WorkoutPlan {
  id: string;
  name: string;
  description?: string;
  exerciseCount: number;
}

export interface WorkoutPlanDetail {
  id: string;
  name: string;
  description?: string;
  exercises: WorkoutPlanExercise[];
}

export interface WorkoutPlanExercise {
  id: string;
  exerciseId: string;
  exerciseName: string;
  muscleGroup: string;
  sets: number;
  reps: number;
  restSeconds: number;
  orderIndex: number;
}

export interface Workout {
  id: string;
  date: string;
  workoutPlanId?: string;
  workoutPlanName?: string;
  durationMinutes: number;
  caloriesBurned: number;
  notes?: string;
  setCount: number;
}

export interface WorkoutDetail {
  id: string;
  date: string;
  workoutPlanId?: string;
  workoutPlanName?: string;
  durationMinutes: number;
  caloriesBurned: number;
  notes?: string;
  sets: WorkoutSet[];
}

export interface WorkoutSet {
  id: string;
  exerciseId: string;
  exerciseName: string;
  muscleGroup: string;
  setNumber: number;
  reps: number;
  weight: number;
  isPersonalRecord: boolean;
}

export const exercisesApi = {
  getAll: (params?: { search?: string; muscleGroup?: string }) => {
    const query = new URLSearchParams();
    if (params?.search) query.set('search', params.search);
    if (params?.muscleGroup) query.set('muscleGroup', params.muscleGroup);
    return api.get<Exercise[]>(`/exercises?${query}`);
  },
  create: (data: { name: string; muscleGroup: string; description?: string; caloriesPerMinute: number }) =>
    api.post<Exercise>('/exercises', data),
};

export const workoutPlansApi = {
  getAll: () => api.get<WorkoutPlan[]>('/workout-plans'),
  getById: (planId: string) => api.get<WorkoutPlanDetail>(`/workout-plans/${planId}`),
  create: (data: { name: string; description?: string; exercises: { exerciseId: string; sets: number; reps: number; restSeconds: number; orderIndex: number }[] }) =>
    api.post<WorkoutPlanDetail>('/workout-plans', data),
  delete: (planId: string) => api.delete(`/workout-plans/${planId}`),
};

export const workoutsApi = {
  getAll: (days?: number) => api.get<Workout[]>(`/workouts${days ? `?days=${days}` : ''}`),
  getById: (workoutId: string) => api.get<WorkoutDetail>(`/workouts/${workoutId}`),
  log: (data: { date: string; workoutPlanId?: string; durationMinutes: number; caloriesBurned: number; notes?: string; sets: { exerciseId: string; setNumber: number; reps: number; weight: number }[] }) =>
    api.post<WorkoutDetail>('/workouts', data),
  delete: (workoutId: string) => api.delete(`/workouts/${workoutId}`),
};
