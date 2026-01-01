// FILE: frontend_harness/src/components/SendCommunicationModal.tsx

import React, { useState, useEffect } from "react";
import { templates, scheduleEmail, sendSms } from "../apiClient";
import type { TemplateDto, ContactDto, VendorDto } from "../types";
import {
  RecipientType,
  TemplateType,
  LeadStage,
  ConnectionStatus,
} from "../types";
import axios from "axios";

// --- HELPER: Vendor -> ContactDto Transformer ---
const mapVendorToContact = (vendor: VendorDto): ContactDto => {
  return {
    id: vendor.id,
    firstName: vendor.name,
    lastName: null,
    email: vendor.email || "",
    phoneNumber: vendor.phoneNumber,
    organization: vendor.serviceType,
    title: "Vendor",
    stage: LeadStage.NewLead,
    connectionStatus: ConnectionStatus.Unknown,
    isLead: false,
    followUpCount: 0,
    lastContactedAt: null,
    source: "Vendor List",
    role: "Vendor",
  };
};

interface SendCommunicationModalProps {
  isOpen: boolean;
  onClose: () => void;
  availableContacts: ContactDto[];
  availableVendors: VendorDto[];
}

export const SendCommunicationModal: React.FC<SendCommunicationModalProps> = ({
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
  const [scheduleTime, setScheduleTime] = useState<string>("");
  const [isSending, setIsSending] = useState(false);

  // Helper to check if current template is SMS
  const isSmsSelected =
    templateList.find((t) => t.id === selectedTemplateId)?.type ===
    TemplateType.SMS;

  useEffect(() => {
    if (isOpen) {
      loadTemplates();
      setSelectedTemplateId("");
      setSelectedRecipientId("");
      setScheduleTime("");
      setRecipientType(RecipientType.Contact);
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

    // 1. Find Template
    const selectedTemplate = templateList.find(
      (t) => t.id === selectedTemplateId
    );
    if (!selectedTemplate) {
      alert("Selected template data not found.");
      return;
    }

    // 2. Prepare Contact Payload
    let contactPayload: ContactDto;

    if (recipientType === RecipientType.Contact) {
      const contact = availableContacts.find(
        (c) => c.id === selectedRecipientId
      );
      if (!contact) {
        alert("Contact not found");
        return;
      }
      contactPayload = contact;
    } else {
      const vendor = availableVendors.find((v) => v.id === selectedRecipientId);
      if (!vendor) {
        alert("Vendor not found");
        return;
      }
      contactPayload = mapVendorToContact(vendor);
    }

    setIsSending(true);

    try {
      // 3. TRAFFIC CONTROL: Check Template Type
      if (selectedTemplate.type === TemplateType.SMS) {
        // --- ROUTE A: SEND SMS ---
        // SMS is currently immediate (not scheduled)
        await sendSms(contactPayload.id, selectedTemplateId);
        alert("SMS sent successfully!");
      } else {
        // --- ROUTE B: SCHEDULE EMAIL ---
        const safeScheduleTime =
          scheduleTime || new Date(Date.now() + 60 * 1000).toISOString();

        await scheduleEmail({
          contact: contactPayload,
          templateId: selectedTemplateId,
          templateContent: selectedTemplate.content || "",
          subject: selectedTemplate.subject || "New Message",
          scheduleTime: safeScheduleTime,
        });
        alert("Email scheduled successfully!");
      }

      onClose();
    } catch (error: unknown) {
      console.error("Failed to send communication", error);

      let errorMessage = "Check console for details.";
      if (
        axios.isAxiosError(error) &&
        error.response?.data?.errors?.[0]?.ErrorMessage
      ) {
        errorMessage = error.response.data.errors[0].ErrorMessage;
      } else if (error instanceof Error) {
        errorMessage = error.message;
      }

      alert(`Failed to send: ${errorMessage}`);
    } finally {
      setIsSending(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-70 flex items-center justify-center z-50 backdrop-blur-sm">
      <div className="bg-gray-900 border border-gray-700 p-6 rounded-xl shadow-2xl w-full max-w-md text-gray-100">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-xl font-bold text-white">Send Communication</h2>
          <button onClick={onClose} className="text-gray-400 hover:text-white">
            ✕
          </button>
        </div>

        {/* Template Selector */}
        <div className="mb-5">
          <label className="block text-xs font-bold text-gray-400 uppercase mb-2">
            Select Template
          </label>
          <select
            className="w-full bg-gray-800 border border-gray-600 p-3 rounded-lg text-white focus:ring-2 focus:ring-blue-500 outline-none"
            value={selectedTemplateId}
            onChange={(e) => setSelectedTemplateId(e.target.value)}>
            <option value="">-- Choose a Template --</option>
            {templateList.map((t) => (
              <option key={t.id} value={t.id}>
                {t.name} ({t.type === TemplateType.Email ? "Email" : "SMS"})
              </option>
            ))}
          </select>
        </div>

        {/* Recipient Type Toggle */}
        <div className="mb-4 bg-gray-800 p-1 rounded-lg flex">
          <button
            onClick={() => {
              setRecipientType(RecipientType.Contact);
              setSelectedRecipientId("");
            }}
            className={`flex-1 py-2 rounded-md text-sm font-medium transition-all ${
              recipientType === RecipientType.Contact
                ? "bg-blue-600 text-white shadow"
                : "text-gray-400 hover:text-white"
            }`}>
            Contact
          </button>
          <button
            onClick={() => {
              setRecipientType(RecipientType.Vendor);
              setSelectedRecipientId("");
            }}
            className={`flex-1 py-2 rounded-md text-sm font-medium transition-all ${
              recipientType === RecipientType.Vendor
                ? "bg-purple-600 text-white shadow"
                : "text-gray-400 hover:text-white"
            }`}>
            Vendor
          </button>
        </div>

        {/* Recipient Selector */}
        <div className="mb-5">
          <label className="block text-xs font-bold text-gray-400 uppercase mb-2">
            Select{" "}
            {recipientType === RecipientType.Contact ? "Contact" : "Vendor"}
          </label>
          <select
            className="w-full bg-gray-800 border border-gray-600 p-3 rounded-lg text-white focus:ring-2 focus:ring-blue-500 outline-none"
            value={selectedRecipientId}
            onChange={(e) => setSelectedRecipientId(e.target.value)}>
            <option value="">-- Choose Recipient --</option>
            {recipientType === RecipientType.Contact
              ? availableContacts.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.firstName} {c.lastName}
                  </option>
                ))
              : availableVendors.map((v) => (
                  <option key={v.id} value={v.id}>
                    {v.name} ({v.serviceType})
                  </option>
                ))}
          </select>
        </div>

        {/* Schedule Time (Conditional Render) */}
        {!isSmsSelected && (
          <div className="mb-6">
            <label className="block text-xs font-bold text-gray-400 uppercase mb-2">
              Send At (Email Only)
            </label>
            <input
              type="datetime-local"
              className="w-full bg-gray-800 border border-gray-600 p-3 rounded-lg text-white focus:ring-2 focus:ring-blue-500 outline-none"
              value={scheduleTime}
              onChange={(e) => setScheduleTime(e.target.value)}
            />
            <p className="text-xs text-gray-500 mt-1">
              Leave blank to send immediately (Autosets to +1 min).
            </p>
          </div>
        )}

        {isSmsSelected && (
          <div className="mb-6 p-3 bg-blue-900/20 border border-blue-800 rounded text-blue-200 text-sm">
            ℹ️ SMS messages are sent immediately.
          </div>
        )}

        {/* Action Buttons */}
        <div className="flex gap-3 pt-2">
          <button
            onClick={onClose}
            className="flex-1 py-3 bg-gray-800 text-gray-300 hover:bg-gray-700 rounded-lg font-medium transition-colors">
            Cancel
          </button>
          <button
            onClick={handleSend}
            disabled={isSending || !selectedTemplateId || !selectedRecipientId}
            className="flex-1 py-3 bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-500 hover:to-indigo-500 text-white rounded-lg font-bold shadow-lg disabled:opacity-50 disabled:cursor-not-allowed transition-all">
            {isSending ? "Confirm & Send" : "Confirm & Send"}
          </button>
        </div>
      </div>
    </div>
  );
};
