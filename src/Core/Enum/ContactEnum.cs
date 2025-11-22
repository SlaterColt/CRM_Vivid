namespace CRM_Vivid.Core.Enum;

public enum ConnectionStatus
{
  // Default state
  Unknown = 0,

  // Diara's specific workflow
  NeedToMeet = 1,
  MetAlready = 2,
  DoesntNeed = 3,

  // "Have/Need" Matrix
  NeedAndDoesntHave = 4,
  NeedAndHas = 5,
  HasAndNeedsToMeet = 6
}

public enum LeadStage
{
  NewLead = 0,         // Just entered system (Intake)
  InDiscussion = 1,    // "Discussion Tab"
  ProposalSent = 2,    // Contract sent
  Negotiating = 3,     // Redlining
  Won = 4,             // Confirmed Client
  Lost = 5             // Dead lead
}