import axios from 'axios';
import type { Document, EventFinancials } from './types'; // Added EventFinancials

// Define the base URL of your local API
const API_BASE_URL = 'http://localhost:5179'; 

export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Updated Upload Function with Category
export const uploadDocument = async (
  file: File, 
  relatedEntityId: string, 
  relatedEntityType: string,
  category: string
): Promise<Document> => {
  const formData = new FormData();
  formData.append('file', file);
  formData.append('relatedEntityId', relatedEntityId);
  formData.append('relatedEntityType', relatedEntityType);
  formData.append('category', category);

  const response = await apiClient.post<Document>('/api/documents', formData, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
  });

  return response.data;
};

// The Shredder Function
export const deleteDocument = async (id: number): Promise<void> => {
  await apiClient.delete(`/api/documents/${id}`);
};

// --- NEW: Financials (The Ledger) ---

export const getEventFinancials = async (eventId: string): Promise<EventFinancials> => {
  const response = await apiClient.get<EventFinancials>(`/api/events/${eventId}/financials`);
  return response.data;
};

export const upsertBudget = async (
  eventId: string, 
  totalAmount: number, 
  currency: string, 
  notes?: string
): Promise<string> => {
  const payload = { eventId, totalAmount, currency, notes };
  const response = await apiClient.post<string>(`/api/events/${eventId}/financials/budget`, payload);
  return response.data;
};

export const addExpense = async (
  eventId: string,
  description: string,
  amount: number,
  dateIncurred: string, // ISO String
  category: string,
  vendorId?: string,
  linkedDocumentId?: number
): Promise<string> => {
  const payload = { eventId, description, amount, dateIncurred, category, vendorId, linkedDocumentId };
  const response = await apiClient.post<string>(`/api/events/${eventId}/financials/expenses`, payload);
  return response.data;
};