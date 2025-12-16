// =======================================================
// FILE: frontend_harness/src/types.ts
// =======================================================

// --- ENUMS REPLACED WITH CONST OBJECTS (Erasable Syntax Compliant) ---

export const VendorType = {
  Catering: "Catering",
  Venue: "Venue",
  Security: "Security",
  Entertainment: "Entertainment",
  Florist: "Florist",
  Photography: "Photography",
  Videography: "Videography",
  Other: "Other"
} as const;
export type VendorType = typeof VendorType[keyof typeof VendorType];

export const LeadStage = {
  NewLead: 0,
  InDiscussion: 1,
  ProposalSent: 2,
  Negotiating: 3,
  Won: 4,
  Lost: 5
} as const;
export type LeadStage = typeof LeadStage[keyof typeof LeadStage];

export const ConnectionStatus = {
  Unknown: 0,
  NeedToMeet: 1,
  MetAlready: 2,
  DoesntNeed: 3,
  NeedAndDoesntHave: 4,
  NeedAndHas: 5,
  HasAndNeedsToMeet: 6
} as const;
export type ConnectionStatus = typeof ConnectionStatus[keyof typeof ConnectionStatus];

export const TaskStatus = {
  NotStarted: "NotStarted",
  InProgress: "InProgress",
  Completed: "Completed",
  Deferred: "Deferred"
} as const;
export type TaskStatus = typeof TaskStatus[keyof typeof TaskStatus];

export const TaskPriority = {
  Low: "Low",
  Medium: "Medium",
  High: "High",
  Urgent: "Urgent"
} as const;
export type TaskPriority = typeof TaskPriority[keyof typeof TaskPriority];

// --- DTOs ---

export interface ContactDto {
  id: string;
  firstName: string;
  lastName: string | null;
  email: string;
  phoneNumber: string | null;
  title: string | null;
  organization: string | null;
  
  // --- NEW: Pipeline Fields (Phase 25) ---
  stage: LeadStage;
  connectionStatus: ConnectionStatus;
  isLead: boolean;
  followUpCount: number;
  lastContactedAt: string | null; // ISO Date
  source: string | null;
}

export interface VendorDto {
  id: string;
  name: string;
  phoneNumber: string | null;
  email: string | null;
  serviceType: VendorType;
  attributes: string | null;
}

export interface EventDto {
  id: string;
  name: string;
  description: string | null;
  startDateTime: string; // ISO 8601 string
  endDateTime: string; // ISO 8601 string
  location: string | null;
  isPublic: boolean;
  status: string; 
}

export interface TaskDto {
  id: string;
  title: string;
  description: string | null;
  status: TaskStatus;
  priority: TaskPriority;
  dueDate: string | null;
  createdAt: string;
  contactId: string | null;
  eventId: string | null;
  vendorId: string | null;
  vendorName: string | null;
  contactEmail?: string;
  eventName?: string;
  vendorNameForLookup?: string;
}

export interface NoteDto {
  id: string;
  content: string;
  createdAt: string;
  updatedAt: string | null;
  contactId: string | null;
  eventId: string | null;
  taskId: string | null;
  vendorId: string | null;
  vendorName: string | null;
  contactEmail?: string;
  eventName?: string;
  vendorNameForLookup?: string;
  taskTitle?: string;
}

export interface TemplateDto {
  id: string;
  name: string;
  subject: string | null;
  content: string;
  type: string;
}

export interface EmailLogDto {
  id: string;
  to: string;
  subject: string;
  sentAt: string;
  isSuccess: boolean;
  errorMessage: string | null;
}

// --- NEW: DASHBOARD DTO (PHASE 31 FIX) ---
export interface DashboardStatsDto {
  totalContacts: number;
  activeEvents: number;
  pendingTasks: number;
  recentEmails: number;
  upcomingEvents: EventDto[]; // Uses the EventDto defined above
}

// --- DOCUMENT & FINANCIALS (Phase 26) ---

export type ContractStatus = 1 | 2 | 3 | 4 | 5;

export const ContractStatusName: Record<ContractStatus, string> = {
    1: "Draft",
    2: "Sent",
    3: "Viewed",
    4: "Signed",
    5: "Voided"
};

export interface Document {
  id: number;
  fileName: string;
  contentType: string;
  size: number;
  uploadedAt: string;
  url: string;
  relatedEntityId: string;
  relatedEntityType: string;
  category: string;
  status: ContractStatus;
  signedAt: string | null;
}

export type ExpenseCategory = 
    | "General" | "Venue" | "Catering" | "Talent" 
    | "Production" | "Marketing" | "Travel" | "Admin" | "Legal";

export interface Expense {
    id: string;
    budgetId: string;
    description: string;
    amount: number;
    dateIncurred: string;
    category: ExpenseCategory;
    vendorId?: string;
    vendorName?: string;
    linkedDocumentId?: number;
    linkedDocumentName?: string;
}

export interface EventFinancials {
    eventId: string;
    eventName: string;
    budgetTotal: number;
    currency: string;
    notes?: string;
    isLocked: boolean;
    expenses: Expense[];
    totalSpent: number;
    remainingBudget: number;
    burnRate: number;
}

// --- COMMANDS ---

export const RecipientType = {
  Contact: 0,
  Vendor: 1
} as const;
export type RecipientType = typeof RecipientType[keyof typeof RecipientType];

export interface SendTemplateEmailCommand {
  eventId: string;
  templateId: string;
  targetEntityId: string;
  recipientType: RecipientType;
}

export interface SubmitLeadCommand {
  firstName: string;
  lastName?: string;
  email: string;
  phoneNumber?: string;
  organization?: string;
  source?: string;
}

// Type aliases for CREATE DTOs
export type CreateContactDto = Omit<ContactDto, 'id'>;
export type CreateEventDto = Omit<EventDto, 'id' | 'createdAt'>;
export type CreateTaskDto = Omit<TaskDto, 'id' | 'createdAt'>;
export type CreateVendorDto = Omit<VendorDto, 'id' | 'createdAt'>;
export type CreateNoteDto = Omit<NoteDto, 'id' | 'createdAt' | 'updatedAt'>;
export type CreateTemplateDto = Omit<TemplateDto, 'id'>;

export type Guid = string;