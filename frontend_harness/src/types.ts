// --- "SOURCE OF TRUTH" DTOs ---

export interface ContactDto {
  id: string;
  firstName: string;
  lastName: string | null;
  email: string; 
  phoneNumber: string | null;
  title: string | null;
  organization: string | null;
}

export interface EventDto {
  id: string;
  name: string;
  description: string | null;
  startDateTime: string; // ISO 8601 string
  endDateTime: string; // ISO 8601 string
  location: string | null;
  isPublic: boolean;
  status: string; // "Planned", "InProgress", "Completed", "Cancelled" etc.
}

export interface TaskDto {
  id: string;
  title: string;
  description: string | null;
  status: string; // "NotStarted", "InProgress", "Completed"
  priority: string; // "Low", "Medium", "High"
  dueDate: string | null; // ISO 8601 string
  createdAt: string; // ISO 8601 string
  contactId: string | null;
  eventId: string | null;
  vendorId: string | null; 
}

export interface VendorDto {
  id: string;
  name: string;
  phoneNumber: string | null;
  email: string | null;
  serviceType: string; 
}

export interface NoteDto {
  id: string;
  content: string;
  createdAt: string; // ISO 8601 string
  updatedAt: string | null; // ISO 8601 string
  contactId: string | null;
  eventId: string | null;
  taskId: string | null;
  vendorId: string | null;
}

export interface TemplateDto {
  id: string;
  name: string;
  subject: string | null;
  content: string;
  type: string; // "Email" or "SMS"
}

export interface EmailLogDto {
  id: string;
  to: string;
  subject: string;
  sentAt: string; // ISO 8601 string
  isSuccess: boolean;
  errorMessage: string | null;
}

// --- Create/Update DTO Types (Omitting 'id') ---
export type CreateContactDto = Omit<ContactDto, 'id' | 'createdAt'>;
export type CreateEventDto = Omit<EventDto, 'id' | 'createdAt'>;
export type CreateTaskDto = Omit<TaskDto, 'id' | 'createdAt'>;
export type CreateVendorDto = Omit<VendorDto, 'id' | 'createdAt'>;
export type CreateNoteDto = Omit<NoteDto, 'id' | 'createdAt' | 'updatedAt'>;
export type CreateTemplateDto = Omit<TemplateDto, 'id'>;

// Type alias for GUIDs used across the application
export type Guid = string;

// --- DASHBOARD DTO ---
export interface DashboardStatsDto {
  totalContacts: number;
  activeEvents: number;
  pendingTasks: number;
  recentEmails: number;
  upcomingEvents: EventDto[];
}

export interface Document {
  id: number;
  fileName: string;
  contentType: string;
  size: number;
  uploadedAt: string; // ISO Date string
  url: string;
  relatedEntityId: string;
  relatedEntityType: string;
  category: string; // NEW: The Librarian field
}