namespace CRM_Vivid.Core.Enum
{
  public enum ContractStatus
  {
    /// <summary>
    /// The document has been created but not yet sent to the client.
    /// </summary>
    Draft = 1,

    /// <summary>
    /// The document has been successfully sent (e.g., via email/DocuSign).
    /// </summary>
    Sent = 2,

    /// <summary>
    /// The client has opened and viewed the document.
    /// </summary>
    Viewed = 3,

    /// <summary>
    /// The client has executed (signed) the document. This is the trigger for internal completion logic.
    /// </summary>
    Signed = 4,

    /// <summary>
    /// The document has been cancelled, superseded, or is otherwise invalid.
    /// </summary>
    Voided = 5
  }
}