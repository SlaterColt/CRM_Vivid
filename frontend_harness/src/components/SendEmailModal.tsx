import React, { useState, useEffect } from "react";
import { templates } from "../apiClient";
import type {
  TemplateDto,
  ContactDto,
  VendorDto,
  SendTemplateEmailCommand,
} from "../types";
import { RecipientType } from "../types"; // RecipientType is a const object (runtime value), so no 'type' keyword here

interface SendEmailModalProps {
  eventId: string;
  isOpen: boolean;
  onClose: () => void;
  availableContacts: ContactDto[];
  availableVendors: VendorDto[];
}

export const SendEmailModal: React.FC<SendEmailModalProps> = ({
  eventId,
  isOpen,
  onClose,
  availableContacts,
  availableVendors,
}) => {
  const [templateList, setTemplateList] = useState<TemplateDto[]>([]);
  const [selectedTemplateId, setSelectedTemplateId] = useState<string>("");
  const [selectedRecipientId, setSelectedRecipientId] = useState<string>("");
  const [recipientType, setRecipientType] = useState<RecipientType>(
    RecipientType.Contact
  );
  const [isSending, setIsSending] = useState(false);

  useEffect(() => {
    if (isOpen) {
      loadTemplates();
    }
  }, [isOpen]);

  const loadTemplates = async () => {
    try {
      const data = await templates.getAll();
      setTemplateList(data);
    } catch (error) {
      console.error("Failed to load templates", error);
    }
  };

  const handleSend = async () => {
    if (!selectedTemplateId || !selectedRecipientId) return;

    setIsSending(true);
    try {
      const command: SendTemplateEmailCommand = {
        eventId,
        templateId: selectedTemplateId,
        targetEntityId: selectedRecipientId,
        recipientType: recipientType,
      };

      await templates.sendEmail(command);
      alert("Email sent successfully!");
      onClose();
    } catch (error) {
      console.error("Failed to send email", error);
      alert("Failed to send email.");
    } finally {
      setIsSending(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg shadow-xl w-full max-w-md">
        <h2 className="text-xl font-bold mb-4">Send Event Email</h2>

        {/* Template Selection */}
        <div className="mb-4">
          <label className="block text-sm font-medium mb-1">
            Select Template
          </label>
          <select
            className="w-full border p-2 rounded"
            value={selectedTemplateId}
            onChange={(e) => setSelectedTemplateId(e.target.value)}>
            <option value="">-- Choose a Template --</option>
            {templateList.map((t) => (
              <option key={t.id} value={t.id}>
                {t.name}
              </option>
            ))}
          </select>
        </div>

        {/* Recipient Type Toggle */}
        <div className="mb-4 flex gap-4">
          <label className="flex items-center">
            <input
              type="radio"
              name="rtype"
              checked={recipientType === RecipientType.Contact}
              onChange={() => {
                setRecipientType(RecipientType.Contact);
                setSelectedRecipientId("");
              }}
              className="mr-2"
            />
            Contact
          </label>
          <label className="flex items-center">
            <input
              type="radio"
              name="rtype"
              checked={recipientType === RecipientType.Vendor}
              onChange={() => {
                setRecipientType(RecipientType.Vendor);
                setSelectedRecipientId("");
              }}
              className="mr-2"
            />
            Vendor
          </label>
        </div>

        {/* Recipient Selection */}
        <div className="mb-6">
          <label className="block text-sm font-medium mb-1">
            Select{" "}
            {recipientType === RecipientType.Contact ? "Contact" : "Vendor"}
          </label>
          <select
            className="w-full border p-2 rounded"
            value={selectedRecipientId}
            onChange={(e) => setSelectedRecipientId(e.target.value)}>
            <option value="">-- Choose Recipient --</option>
            {recipientType === RecipientType.Contact
              ? availableContacts.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.firstName} {c.lastName} ({c.organization})
                  </option>
                ))
              : availableVendors.map((v) => (
                  <option key={v.id} value={v.id}>
                    {v.name} ({v.serviceType})
                  </option>
                ))}
          </select>
        </div>

        <div className="flex justify-end gap-2">
          <button
            onClick={onClose}
            className="px-4 py-2 text-gray-600 hover:bg-gray-100 rounded">
            Cancel
          </button>
          <button
            onClick={handleSend}
            disabled={isSending || !selectedTemplateId || !selectedRecipientId}
            className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:opacity-50">
            {isSending ? "Sending..." : "Send Email"}
          </button>
        </div>
      </div>
    </div>
  );
};
