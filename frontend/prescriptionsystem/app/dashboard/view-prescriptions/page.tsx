"use client"

import { useState, useEffect } from "react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Checkbox } from "@/components/ui/checkbox"

interface PrescriptionItem {
  id: number
  medicineBarcode: string
  medicineName: string
  quantity: number
  usage: string
  isSubmitted: boolean
}

interface Prescription {
  id: number
  patientTc: string
  doctorId: number
  createdDate: string
  status: number
  items: PrescriptionItem[]
}

export default function ViewPrescriptions() {
  const [patientTc, setPatientTc] = useState("")
  const [prescriptions, setPrescriptions] = useState<Prescription[]>([])
  const [error, setError] = useState("")
  const [successMessage, setSuccessMessage] = useState("")
  const [selectedItems, setSelectedItems] = useState<{ [key: number]: boolean }>({})

  useEffect(() => {
    setSelectedItems({})
  }, [prescriptions])

  const handleSearch = async () => {
    setError("")
    setSuccessMessage("")
    try {
      const response = await fetch(`/api/v1/Prescription/by-tc/?patientTc=${patientTc}`, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      })

      if (response.ok) {
        const data = await response.json()
        setPrescriptions(Array.isArray(data) ? data : [data])
      } else {
        setError("Failed to fetch prescriptions")
      }
    } catch (error) {
      setError("An error occurred. Please try again.")
    }
  }

  const handleSubmit = async (prescriptionId: number, selectedBarcodes: string[]) => {
    console.log("Submitting prescription:", prescriptionId, "with barcodes:", selectedBarcodes)
    setError("")
    setSuccessMessage("")
    try {
      const response = await fetch("/api/v1/Prescription/submit", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
        body: JSON.stringify({ prescriptionId, medicineBarcodes: selectedBarcodes }),
      })

      if (response.ok) {
        // Update the local state to reflect the submitted items
        setPrescriptions(
          prescriptions.map((prescription) => {
            if (prescription.id === prescriptionId) {
              return {
                ...prescription,
                items: prescription.items.map((item) => ({
                  ...item,
                  isSubmitted: selectedBarcodes.includes(item.medicineBarcode) || item.isSubmitted,
                })),
              }
            }
            return prescription
          }),
        )

        setSuccessMessage("The prescription has been successfully submitted.")
      } else {
        setError("Failed to submit the prescription. Please try again.")
      }
    } catch (error) {
      setError("An error occurred while submitting the prescription.")
    }
  }

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle>View Prescriptions</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex space-x-2">
            <Input
              type="text"
              placeholder="Patient TC"
              value={patientTc}
              onChange={(e) => setPatientTc(e.target.value)}
            />
            <Button onClick={handleSearch}>Search</Button>
          </div>
          {error && <p className="text-red-500 mt-2">{error}</p>}
          {successMessage && <p className="text-green-500 mt-2">{successMessage}</p>}
        </CardContent>
      </Card>

      {prescriptions.map((prescription) => (
        <Card key={prescription.id}>
          <CardHeader>
            <CardTitle>Prescription ID: {prescription.id}</CardTitle>
          </CardHeader>
          <CardContent>
            <p>Patient TC: {prescription.patientTc}</p>
            <p>Created Date: {new Date(prescription.createdDate).toLocaleString()}</p>
            <h3 className="font-bold mt-4 mb-2">Medicines:</h3>
            <form
              onSubmit={(e) => {
                e.preventDefault()
                const selectedBarcodes = prescription.items
                  .filter((item) => !item.isSubmitted && selectedItems[item.id])
                  .map((item) => item.medicineBarcode)
                if (selectedBarcodes.length > 0) {
                  handleSubmit(prescription.id, selectedBarcodes)
                } else {
                  setError("Please select at least one medicine to submit.")
                }
              }}
            >
              <ul className="space-y-2">
                {prescription.items.map((item) => (
                  <li key={item.id} className="flex items-center space-x-2">
                    <Checkbox
                      id={`item-${item.id}`}
                      checked={selectedItems[item.id] || false}
                      onCheckedChange={(checked) => {
                        setSelectedItems((prev) => ({ ...prev, [item.id]: checked }))
                      }}
                      disabled={item.isSubmitted}
                    />
                    <label htmlFor={`item-${item.id}`} className={item.isSubmitted ? "line-through" : ""}>
                      {item.medicineName} - Quantity: {item.quantity}, Usage: {item.usage}
                    </label>
                  </li>
                ))}
              </ul>
              <Button type="submit" className="mt-4">
                Submit Selected
              </Button>
            </form>
          </CardContent>
        </Card>
      ))}
    </div>
  )
}

