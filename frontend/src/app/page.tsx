'use client';

import Link from 'next/link';
import { useAuth } from '@/contexts/AuthContext';

export default function HomePage() {
  const { isAuthenticated, user } = useAuth();

  return (
    <div className="min-h-screen bg-gradient-to-br from-green-50 to-emerald-100">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex justify-between items-center">
          <div className="flex items-center gap-2">
            <span className="text-2xl">🍽️</span>
            <span className="text-xl font-bold text-green-700">MealMate</span>
          </div>
          <div className="flex gap-4 items-center">
            {isAuthenticated ? (
              <>
                <Link href="/dashboard" className="text-gray-600 hover:text-green-700 font-medium">
                  Dashboard
                </Link>
                <span className="text-gray-500">|</span>
                <span className="text-gray-700">Hi, {user?.firstName}!</span>
              </>
            ) : (
              <>
                <Link href="/login" className="text-gray-600 hover:text-green-700 font-medium">
                  Login
                </Link>
                <Link href="/register" className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors">
                  Get Started
                </Link>
              </>
            )}
          </div>
        </div>
      </nav>

      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-20">
        <div className="text-center">
          <h1 className="text-5xl font-bold text-gray-900 mb-6">
            Your Smart Household<br />
            <span className="text-green-600">Meal Planning Hub</span>
          </h1>
          <p className="text-xl text-gray-600 mb-10 max-w-2xl mx-auto">
            Plan meals, track your fridge, manage shopping lists, count calories,
            and track fitness all together with your household.
          </p>
          <div className="flex gap-4 justify-center">
            <Link href="/register" className="bg-green-600 text-white px-8 py-4 rounded-xl text-lg font-semibold hover:bg-green-700 transition-colors shadow-lg">
              Start for Free
            </Link>
            <Link href="/recipes" className="bg-white text-green-700 px-8 py-4 rounded-xl text-lg font-semibold hover:bg-green-50 transition-colors shadow-lg border border-green-200">
              Browse Recipes
            </Link>
          </div>
        </div>

        <div className="mt-24 grid grid-cols-1 md:grid-cols-3 gap-8">
          {[
            { icon: '🍽️', title: 'Meal Planning', desc: 'Plan your weekly meals. Auto-generate shopping lists.' },
            { icon: '🧊', title: 'Smart Fridge', desc: 'Track what is in your fridge. Get alerts when items are about to expire.' },
            { icon: '🛒', title: 'Shopping Lists', desc: 'Collaborative shopping lists. Mark items as bought and update the fridge automatically.' },
            { icon: '🔥', title: 'Calorie Tracking', desc: 'Log meals and track macros. Set daily goals and see your progress.' },
            { icon: '🏋️', title: 'Fitness Tracker', desc: 'Log workouts, track personal records, and see your fitness progress.' },
            { icon: '👨‍👩‍👧‍👦', title: 'Household Sharing', desc: 'Invite family members with a secure code. Share everything together.' },
          ].map((feature) => (
            <div key={feature.title} className="bg-white p-6 rounded-2xl shadow-sm border border-gray-100 hover:shadow-md transition-shadow">
              <div className="text-4xl mb-4">{feature.icon}</div>
              <h3 className="text-xl font-semibold text-gray-800 mb-2">{feature.title}</h3>
              <p className="text-gray-600">{feature.desc}</p>
            </div>
          ))}
        </div>
      </main>
    </div>
  );
}
