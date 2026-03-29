'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { useAuth } from '@/contexts/AuthContext';
import { householdsApi, HouseholdDetail } from '@/services/api';

export default function HouseholdsPage() {
  const { isAuthenticated, isLoading } = useAuth();
  const router = useRouter();
  const [households, setHouseholds] = useState<HouseholdDetail[]>([]);
  const [loading, setLoading] = useState(true);
  const [newName, setNewName] = useState('');
  const [joinCode, setJoinCode] = useState('');
  const [inviteEmail, setInviteEmail] = useState('');
  const [selectedHousehold, setSelectedHousehold] = useState<string | null>(null);
  const [message, setMessage] = useState<{ text: string; type: 'success' | 'error' } | null>(null);

  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      router.push('/login');
    }
  }, [isLoading, isAuthenticated, router]);

  useEffect(() => {
    if (isAuthenticated) {
      fetchHouseholds();
    }
  }, [isAuthenticated]);

  const fetchHouseholds = async () => {
    const r = await householdsApi.getMyHouseholds();
    if (r.data) {
      setHouseholds(r.data as HouseholdDetail[]);
    }
    setLoading(false);
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    const r = await householdsApi.create({ name: newName });
    if (r.error) {
      setMessage({ text: r.error, type: 'error' });
    } else {
      setMessage({ text: 'Household created!', type: 'success' });
      setNewName('');
      fetchHouseholds();
    }
  };

  const handleJoin = async (e: React.FormEvent) => {
    e.preventDefault();
    const r = await householdsApi.join(joinCode.toUpperCase());
    if (r.error) {
      setMessage({ text: r.error, type: 'error' });
    } else {
      setMessage({ text: 'Joined household!', type: 'success' });
      setJoinCode('');
      fetchHouseholds();
    }
  };

  const handleInvite = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedHousehold) return;
    const r = await householdsApi.invite(selectedHousehold, inviteEmail);
    if (r.error) {
      setMessage({ text: r.error, type: 'error' });
    } else {
      setMessage({ text: 'Invite sent!', type: 'success' });
      setInviteEmail('');
    }
  };

  if (isLoading || loading) return <div className="min-h-screen flex items-center justify-center">Loading...</div>;

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex justify-between items-center">
          <Link href="/dashboard" className="flex items-center gap-2">
            <span className="text-2xl">🍽️</span>
            <span className="text-xl font-bold text-green-700">MealMate</span>
          </Link>
          <Link href="/dashboard" className="text-gray-600 hover:text-green-700">Dashboard</Link>
        </div>
      </nav>

      <main className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <h1 className="text-3xl font-bold text-gray-900 mb-6">My Households</h1>

        {message && (
          <div className={`px-4 py-3 rounded-lg mb-6 text-sm ${
            message.type === 'success' ? 'bg-green-50 text-green-700' : 'bg-red-50 text-red-700'
          }`}>
            {message.text}
          </div>
        )}

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
          <div className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100">
            <h2 className="text-lg font-semibold text-gray-800 mb-4">Create New Household</h2>
            <form onSubmit={handleCreate} className="space-y-3">
              <input
                type="text"
                value={newName}
                onChange={e => setNewName(e.target.value)}
                placeholder="Household name"
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-green-500"
                required
              />
              <button type="submit" className="w-full bg-green-600 text-white py-2 rounded-lg hover:bg-green-700 transition-colors">
                Create
              </button>
            </form>
          </div>

          <div className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100">
            <h2 className="text-lg font-semibold text-gray-800 mb-4">Join with Code</h2>
            <form onSubmit={handleJoin} className="space-y-3">
              <input
                type="text"
                value={joinCode}
                onChange={e => setJoinCode(e.target.value.toUpperCase())}
                placeholder="Enter 5-char code (e.g. A7X2K)"
                maxLength={5}
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-green-500 font-mono uppercase tracking-widest text-center text-lg"
                required
              />
              <button type="submit" className="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition-colors">
                Join
              </button>
            </form>
          </div>
        </div>

        {households.length > 0 ? (
          <div className="space-y-4">
            {households.map(h => (
              <div key={h.id} className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100">
                <div className="flex justify-between items-start">
                  <div>
                    <h3 className="text-xl font-semibold text-gray-800">{h.name}</h3>
                    <p className="text-gray-500 text-sm mt-1">Created by {h.createdByUserName} · {h.memberCount} member(s)</p>
                  </div>
                  <button
                    onClick={() => setSelectedHousehold(selectedHousehold === h.id ? null : h.id)}
                    className="text-sm text-green-600 hover:underline"
                  >
                    {selectedHousehold === h.id ? 'Hide' : 'Invite Member'}
                  </button>
                </div>

                {selectedHousehold === h.id && (
                  <form onSubmit={handleInvite} className="mt-4 flex gap-2">
                    <input
                      type="email"
                      value={inviteEmail}
                      onChange={e => setInviteEmail(e.target.value)}
                      placeholder="email@example.com"
                      className="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-green-500 text-sm"
                      required
                    />
                    <button type="submit" className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 text-sm">
                      Send Invite
                    </button>
                  </form>
                )}
              </div>
            ))}
          </div>
        ) : (
          <div className="text-center py-20 text-gray-400">
            You have no households yet. Create one or join with a code!
          </div>
        )}
      </main>
    </div>
  );
}
