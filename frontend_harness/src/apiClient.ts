import axios from 'axios';
import type { Document } from './types'; // Fix: Type-only import

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
  category: string // NEW ARGUMENT
): Promise<Document> => {
  const formData = new FormData();
  formData.append('file', file);
  formData.append('relatedEntityId', relatedEntityId);
  formData.append('relatedEntityType', relatedEntityType);
  formData.append('category', category); // Append the tag

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