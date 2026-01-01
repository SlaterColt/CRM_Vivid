// FILE: frontend_harness/src/apiClient.ts (COMPLETE REPLACEMENT)
import axios from 'axios';
import { 
  type EventFinancialsDto, 
  type SendTemplateEmailCommand,
  type TemplateDto, 
  type ContactDto,
  type VendorDto,
  type EventDto,
  type TaskDto,
  type NoteDto,
  type EmailLogDto,
  type SubmitLeadCommand,
  RecipientType,
  type DocumentDto,
} from './types';

// Define the base URL of your local API
const API_BASE_URL = 'http://localhost:5179';

export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// =======================================================
// CORE SERVICES
// =======================================================

// --- DOCUMENT SERVICES ---

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

export const deleteDocument = async (id: number): Promise<void> => {
  await apiClient.delete(`/api/documents/${id}`);
};

// --- FINANCIALS SERVICES ---

export const getEventFinancials = async (eventId: string): Promise<EventFinancialsDto> => {
  const response = await apiClient.get<EventFinancialsDto>(`/api/events/${eventId}/financials`);
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


// --- TEMPLATE SERVICES ---

export const templates = {
  getAll: async () => {
    const response = await apiClient.get<TemplateDto[]>('/api/templates');
    return response.data;
  },
  create: async (data: unknown) => {
    const response = await apiClient.post<string>('/api/templates', data);
    return response.data;
  },
  sendEmail: async (command: SendTemplateEmailCommand) => {
    // Uses POST /api/templates/send
    await apiClient.post('/api/templates/send', command);
  }
};

/**
 * Initializes required contacts, vendors, and events for testing.
 * @returns The ID of the primary contact created.
 */
export const initializeTestFixtures = async (): Promise<string> => {
  const response = await apiClient.post<string>('/api/contacts/init-fixtures');
  return response.data;
};

// =======================================================
// FULL CRUD IMPLEMENTATION (PHASE 31)
// =======================================================

// --- CONTACTS CRUD & RELATIONS ---

export const updateContact = async (id: string, data: Partial<ContactDto>): Promise<void> => {
  await apiClient.put(`/api/contacts/${id}`, data);
};

export const deleteContact = async (id: string): Promise<void> => {
  await apiClient.delete(`/api/contacts/${id}`);
};

export const getContactEmailLogs = async (contactId: string): Promise<EmailLogDto[]> => {
  const response = await apiClient.get<EmailLogDto[]>(`/api/contacts/${contactId}/email-logs`);
  return response.data;
};

// --- EVENTS CRUD & RELATIONS ---

export const updateEvent = async (id: string, data: Partial<EventDto>): Promise<void> => {
  await apiClient.put(`/api/events/${id}`, data);
};

export const deleteEvent = async (id: string): Promise<void> => {
  await apiClient.delete(`/api/events/${id}`);
};

export const unlinkContactFromEvent = async (eventId: string, contactId: string): Promise<void> => {
  await apiClient.delete(`/api/events/${eventId}/contacts/${contactId}`);
};


// --- VENDORS CRUD ---

export const updateVendor = async (id: string, data: Partial<VendorDto>): Promise<void> => {
  await apiClient.put(`/api/vendors/${id}`, data);
};

export const deleteVendor = async (id: string): Promise<void> => {
  await apiClient.delete(`/api/vendors/${id}`);
};


// --- TASKS CRUD ---
// (TasksController only has Create/Update/Delete - Reads are handled via GetTasksQuery)
export const updateTask = async (id: string, data: Partial<TaskDto>): Promise<void> => {
  await apiClient.put(`/api/tasks/${id}`, data);
};

export const deleteTask = async (id: string): Promise<void> => {
  await apiClient.delete(`/api/tasks/${id}`);
};


// --- NOTES CRUD ---
// (NotesController only has Update/Delete - Create returns DTO)
export const updateNote = async (id: string, data: Partial<NoteDto>): Promise<void> => {
  // NOTE: Notes only allow updating 'content' per the business logic
  await apiClient.put(`/api/notes/${id}`, data); 
};

export const deleteNote = async (id: string): Promise<void> => {
  await apiClient.delete(`/api/notes/${id}`);
};


// --- AUTOMATION & LEADS ---

export const submitLead = async (command: SubmitLeadCommand): Promise<string> => {
  const response = await apiClient.post<string>('/api/leads/submit', command);
  return response.data;
};

export const scheduleFollowUp = async (
  contactId: string, 
  templateId: string, 
  scheduleTime: string, // ISO String
  type: RecipientType = RecipientType.Contact
): Promise<string> => {
  const payload = { contactId, templateId, scheduleTime, type };
  const response = await apiClient.post<string>('/api/automation/schedule-followup', payload);
  return response.data;
};

// FIXED: Interface matches Backend expectation of 'TemplateContent'
export interface ScheduleEmailCommand {
  contact: ContactDto;
  templateId?: string;
  subject: string;
  templateContent: string; // RENAMED from 'body' to match backend property
  scheduleTime: string;
}

export const scheduleEmail = async (command: ScheduleEmailCommand): Promise<void> => {
  // DIAGNOSTIC LOG (You can remove this later)
  console.log("🚀 SENDING PAYLOAD:", JSON.stringify(command, null, 2)); 
  await apiClient.post('/api/automation/schedule-email', command);
};

export const generateContract = async (eventId: string): Promise<DocumentDto> => {
  // This endpoint will return the metadata of the created file
  const response = await apiClient.post<DocumentDto>(`/api/documents/generate-contract/${eventId}`);
  return response.data;
};

export const sendSms = async (contactId: string, templateId: string): Promise<void> => {
  // This hits the AutomationController endpoint for SMS
  await apiClient.post('/api/automation/send-sms', { 
    contactId: contactId, 
    templateId: templateId 
  });
};