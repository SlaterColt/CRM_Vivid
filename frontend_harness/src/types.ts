// =======================================================
// FILE: frontend_harness/src/types.ts
// GENERATED FROM BACKEND DUMP - DO NOT MODIFY MANUALLY
// =======================================================

// --- ENUMS (String-based via JsonStringEnumConverter) ---

export const TemplateType = {
  Email: "Email",
  SMS: "SMS"
} as const;
export type TemplateType = typeof TemplateType[keyof typeof TemplateType];

export const ExpenseCategory = {
  General: "General",
  Venue: "Venue",
  Catering: "Catering",
  Talent: "Talent",
  Production: "Production",
  Marketing: "Marketing",
  Travel: "Travel",
  Admin: "Admin",
  Legal: "Legal"
} as const;
export type ExpenseCategory = typeof ExpenseCategory[keyof typeof ExpenseCategory];

export const EventStatus = {
  Planned: "Planned",
  InProgress: "InProgress",
  Completed: "Completed",
  Postponed: "Postponed",
  Cancelled: "Cancelled"
} as const;
export type EventStatus = typeof EventStatus[keyof typeof EventStatus];

export const TaskPriority = {
  Low: "Low",
  Medium: "Medium",
  High: "High",
  Urgent: "Urgent"
} as const;
export type TaskPriority = typeof TaskPriority[keyof typeof TaskPriority];

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

export const ContractStatus = {
  Draft: "Draft",
  Sent: "Sent",
  Viewed: "Viewed",
  Signed: "Signed",
  Voided: "Voided"
} as const;
export type ContractStatus = typeof ContractStatus[keyof typeof ContractStatus];

export const ConnectionStatus = {
  Unknown: "Unknown",
  NeedToMeet: "NeedToMeet",
  MetAlready: "MetAlready",
  DoesntNeed: "DoesntNeed",
  NeedAndDoesntHave: "NeedAndDoesntHave",
  NeedAndHas: "NeedAndHas",
  HasAndNeedsToMeet: "HasAndNeedsToMeet"
} as const;
export type ConnectionStatus = typeof ConnectionStatus[keyof typeof ConnectionStatus];

export const LeadStage = {
  NewLead: "NewLead",
  InDiscussion: "InDiscussion",
  ProposalSent: "ProposalSent",
  Negotiating: "Negotiating",
  Won: "Won",
  Lost: "Lost"
} as const;
export type LeadStage = typeof LeadStage[keyof typeof LeadStage];

export const TaskStatus = {
  NotStarted: "NotStarted",
  InProgress: "InProgress",
  Completed: "Completed",
  Deferred: "Deferred"
} as const;
export type TaskStatus = typeof TaskStatus[keyof typeof TaskStatus];

export const RecipientType = {
  Contact: "Contact",
  Vendor: "Vendor"
} as const;
export type RecipientType = typeof RecipientType[keyof typeof RecipientType];

// --- READ DTOs (from Application/Common/Models) ---

export interface ActivityDto {
  id: string;
  timestamp: string; // DateTime
  activityType: string;
  title: string;
  content: string | null;
  status: string | null;
  relatedEntityId: string | null;
}

export interface ContactDto {
  id: string;
  firstName: string;
  lastName: string | null;
  email: string;
  phoneNumber: string | null;
  title: string | null;
  organization: string | null;
  // Pipeline Fields
  stage: LeadStage;
  connectionStatus: ConnectionStatus;
  isLead: boolean;
  followUpCount: number;
  lastContactedAt: string | null; // DateTime
  source: string | null;
  // Contextual
  role: string | null;
}

export interface ContactTaskSummaryDto {
  contactId: string;
  contactName: string;
  totalTasksAssigned: number;
  totalTasksCompleted: number;
  totalTasksOverdue: number;
}

export interface DashboardStatsDto {
  totalContacts: number;
  activeEvents: number;
  pendingTasks: number;
  recentEmails: number;
  upcomingEvents: EventDto[];
}

export interface DocumentDto {
  id: number;
  fileName: string;
  contentType: string;
  size: number;
  uploadedAt: string; // DateTime
  url: string;
  relatedEntityId: string;
  relatedEntityType: string;
  category: string;
}

export interface EmailLogDto {
  id: string;
  to: string;
  subject: string;
  sentAt: string; // DateTime
  isSuccess: boolean;
  errorMessage: string | null;
}

export interface EventDto {
  id: string;
  name: string;
  description: string | null;
  startDateTime: string; // DateTime
  endDateTime: string; // DateTime
  location: string | null;
  isPublic: boolean;
  status: string; // Mapped to String
}

export interface ExpenseDto {
  id: string;
  budgetId: string;
  description: string;
  amount: number;
  dateIncurred: string; // DateTime
  category: string; // Mapped to String (ExpenseCategory)
  vendorId: string | null;
  vendorName: string | null;
  linkedDocumentId: number | null;
  linkedDocumentName: string | null;
}

export interface EventFinancialsDto {
  eventId: string;
  eventName: string;
  budgetTotal: number;
  currency: string;
  notes: string | null;
  isLocked: boolean;
  expenses: ExpenseDto[];
  totalSpent: number;
  remainingBudget: number;
  burnRate: number;
}

export interface NoteDto {
  id: string;
  content: string;
  createdAt: string; // DateTime
  updatedAt: string | null; // DateTime
  contactId: string | null;
  eventId: string | null;
  taskId: string | null;
  vendorId: string | null;
  vendorName: string | null;
}

export interface TaskDto {
  id: string;
  title: string;
  description: string | null;
  status: string; // Mapped to String (TaskStatus)
  priority: string; // Mapped to String (TaskPriority)
  dueDate: string | null; // DateTime
  createdAt: string; // DateTime
  contactId: string | null;
  eventId: string | null;
  vendorId: string | null;
  vendorName: string | null;
}

export interface TemplateDto {
  id: string;
  name: string;
  subject: string | null;
  content: string;
  type: string; // Mapped to String (TemplateType)
}

export interface VendorDto {
  id: string;
  name: string;
  phoneNumber: string | null;
  email: string | null;
  serviceType: string; // Mapped to String (VendorType)
  attributes: string | null; // JSON string
  role: string | null;
}

export interface VendorSummaryDto {
  vendorId: string;
  vendorName: string;
  totalEventsHiredFor: number;
  totalExpensesPaid: number;
}

// --- WRITE DTOs (Commands) ---

// Automation
export interface ScheduleFollowUpCommand {
  contactId: string;
  templateId: string;
  scheduleTime: string; // DateTime
  type: RecipientType;
}

export interface SendSmsCommand {
  toPhoneNumber: string;
  body: string;
}

// Contacts
export interface CreateContactCommand {
  firstName: string;
  lastName: string | null;
  email: string;
  phoneNumber: string | null;
  title: string | null;
  organization: string | null;
  stage: LeadStage;
  connectionStatus: ConnectionStatus;
  source: string | null;
}

export interface UpdateContactCommand {
  id: string;
  firstName: string;
  lastName: string | null;
  email: string;
  phoneNumber: string | null;
  title: string | null;
  organization: string | null;
  stage: LeadStage;
  connectionStatus: ConnectionStatus;
  source: string | null;
  incrementFollowUpCount: boolean;
}

export interface SubmitLeadCommand {
  firstName: string;
  lastName: string | null;
  email: string;
  phoneNumber: string | null;
  organization: string | null;
  source: string | null;
}

// Documents
export interface UploadDocumentCommand {
  // Usually handled via FormData, but structured here for reference
  fileName: string;
  contentType: string;
  size: number;
  relatedEntityId: string;
  relatedEntityType: string;
  category: string;
}

// Events
export interface CreateEventCommand {
  name: string;
  startDateTime: string; // DateTime
  endDateTime: string; // DateTime
  isPublic: boolean;
  location: string | null;
  description: string | null;
}

export interface UpdateEventCommand {
  id: string;
  name: string;
  startDateTime: string; // DateTime
  endDateTime: string; // DateTime
  isPublic: boolean;
  location: string | null;
  description: string | null;
  status: EventStatus;
}

export interface AddContactToEventCommand {
  eventId: string;
  contactId: string;
  role: string | null;
}

export interface AddVendorToEventCommand {
  eventId: string;
  vendorId: string;
  role: string | null;
}

// Financials
export interface UpsertBudgetCommand {
  eventId: string;
  totalAmount: number;
  currency: string;
  notes: string | null;
}

export interface AddExpenseCommand {
  eventId: string;
  description: string;
  amount: number;
  dateIncurred: string; // DateTime
  category: string; // String to match Enum parsing in backend
  vendorId: string | null;
  linkedDocumentId: number | null;
}

// Notes
export interface CreateNoteCommand {
  content: string;
  contactId: string | null;
  eventId: string | null;
  taskId: string | null;
  vendorId: string | null;
  // String Lookups
  contactEmail: string | null;
  vendorName: string | null;
  eventName: string | null;
  taskTitle: string | null;
}

export interface UpdateNoteCommand {
  id: string;
  content: string;
}

// Tasks
export interface CreateTaskCommand {
  title: string;
  description: string | null;
  status: TaskStatus;
  priority: TaskPriority;
  dueDate: string | null; // DateTime
  contactId: string | null;
  eventId: string | null;
  vendorId: string | null;
  // String Lookups
  contactEmail: string | null;
  vendorName: string | null;
  eventName: string | null;
}

export interface UpdateTaskCommand {
  id: string;
  title: string;
  description: string | null;
  status: TaskStatus;
  priority: TaskPriority;
  dueDate: string | null; // DateTime
  contactId: string | null;
  eventId: string | null;
  vendorId: string | null;
  // String Lookups
  contactEmail: string | null;
  vendorName: string | null;
  eventName: string | null;
}

// Templates
export interface CreateTemplateCommand {
  name: string;
  subject: string | null;
  content: string;
  type: string;
}

export interface UpdateTemplateCommand {
  id: string;
  name: string;
  subject: string | null;
  content: string;
  type: string;
}

export interface SendTemplateEmailCommand {
  eventId: string;
  templateId: string;
  targetEntityId: string;
  recipientType: RecipientType;
}

// Vendors
export interface CreateVendorCommand {
  name: string;
  phoneNumber: string | null;
  email: string | null;
  serviceType: string;
  attributes: string | null; // JSON string
}

export interface UpdateVendorCommand {
  id: string;
  name: string;
  phoneNumber: string | null;
  email: string | null;
  serviceType: string;
  attributes: string | null; // JSON string
}

// Global Types
export type Guid = string;