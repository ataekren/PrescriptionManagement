"use client"

import { useState, useEffect } from "react"
import { useRouter } from "next/navigation"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from "@/components/ui/command"
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover"

interface PrescriptionItem {
  medicineBarcode: string
  medicineName: string
  quantity: number
  usage: string
  price?: number
}

interface Medicine {
  id: string
  name: string
  barcode: string
  price: number
}

export default function CreatePrescription() {
  const [patientTc, setPatientTc] = useState("")
  const [patientName, setPatientName] = useState("")
  const [items, setItems] = useState<PrescriptionItem[]>([])
  const [error, setError] = useState("")
  const [medicines, setMedicines] = useState<Medicine[]>([])
  const [searchQueries, setSearchQueries] = useState<{ [key: number]: string }>({})
  const router = useRouter()

  useEffect(() => {
    fetchMedicines()
  }, [])

  const fetchMedicines = async () => {
    try {
      const response = await fetch("/api/v1/medicine/active?pageNumber=1&pageSize=10000")
      if (response.ok) {
        const data = await response.json()
        setMedicines(data.items)
      } else {
        setError("Failed to fetch medicines")
      }
    } catch (error) {
      setError("An error occurred while fetching medicines")
    }
  }

  const handlePatientLookup = async () => {
    try {
      const response = await fetch(`/api/v1/Prescription/mock-lookup?tc=${patientTc}`)
      if (response.ok) {
        const data = await response.text()
        setPatientName(data)
      } else {
        setError("Patient not found")
      }
    } catch (error) {
      setError("An error occurred during patient lookup")
    }
  }

  const handleAddItem = () => {
    setItems([...items, { medicineBarcode: "", medicineName: "", quantity: 1, usage: "" }])
  }

  const handleItemChange = (index: number, field: keyof PrescriptionItem, value: string | number) => {
    const newItems = [...items]
    newItems[index] = { ...newItems[index], [field]: field === "quantity" ? Number(value) : value }
    setItems(newItems)
  }

  const handleMedicineSelect = (index: number, medicine: Medicine) => {
    const newItems = [...items]
    newItems[index] = {
      ...newItems[index],
      medicineName: medicine.name,
      medicineBarcode: medicine.barcode,
      price: medicine.price,
    }
    setItems(newItems)
    setSearchQueries({ ...searchQueries, [index]: "" })
    document.body.click() // This will close the popover
  }

  const handleSearchChange = (index: number, value: string) => {
    setSearchQueries({ ...searchQueries, [index]: value })
  }

  const getFilteredMedicines = (index: number) => {
    const query = searchQueries[index]?.toLowerCase() || ""
    return query === "" ? medicines : medicines.filter((medicine) => medicine.name.toLowerCase().includes(query))
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError("")

    try {
      const response = await fetch("/api/v1/Prescription", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
        body: JSON.stringify({ patientTc, items }),
      })

      if (response.ok) {
        router.push("/dashboard")
      } else {
        setError("Failed to create prescription")
      }
    } catch (error) {
      setError("An error occurred. Please try again.")
    }
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>Create Prescription</CardTitle>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="flex space-x-2">
            <Input
              type="text"
              placeholder="Patient TC"
              value={patientTc}
              onChange={(e) => setPatientTc(e.target.value)}
              required
            />
            <Button type="button" onClick={handlePatientLookup}>
              Lookup
            </Button>
          </div>
          {patientName && (
            <div className="p-4 bg-muted rounded-lg">
              <p className="font-medium">Patient: {patientName}</p>
            </div>
          )}
          {items.map((item, index) => (
            <div key={index} className="space-y-2 p-4 border rounded-lg">
              <Popover>
                <PopoverTrigger asChild>
                  <Button variant="outline" role="combobox" className="w-full justify-between">
                    {item.medicineName || "Search medicine..."}
                    <span className="sr-only">Toggle medicine list</span>
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-full p-0">
                  <Command>
                    <CommandInput
                      placeholder="Search medicine..."
                      value={searchQueries[index] || ""}
                      onValueChange={(value) => handleSearchChange(index, value)}
                    />
                    <CommandList>
                      <CommandEmpty>No medicine found.</CommandEmpty>
                      <CommandGroup>
                        {getFilteredMedicines(index).map((medicine) => (
                          <CommandItem
                            key={medicine.id}
                            value={medicine.name}
                            onSelect={() => handleMedicineSelect(index, medicine)}
                          >
                            {medicine.name}
                          </CommandItem>
                        ))}
                      </CommandGroup>
                    </CommandList>
                  </Command>
                </PopoverContent>
              </Popover>
              <div className="grid grid-cols-2 gap-2">
                <Input
                  type="text"
                  placeholder="Medicine Barcode"
                  value={item.medicineBarcode}
                  readOnly
                  className="bg-muted"
                />
                {item.price && (
                  <div className="flex items-center justify-end px-3 py-2 bg-muted rounded-md">
                    <span className="font-medium">Price: ₺{item.price}</span>
                  </div>
                )}
              </div>
              <Input
                type="number"
                placeholder="Quantity"
                value={item.quantity}
                onChange={(e) => handleItemChange(index, "quantity", e.target.value)}
                required
                min="1"
              />
              <Input
                type="text"
                placeholder="Usage (e.g., 1x1)"
                value={item.usage}
                onChange={(e) => handleItemChange(index, "usage", e.target.value)}
                required
              />
            </div>
          ))}
          <div className="flex justify-between">
            <Button type="button" onClick={handleAddItem} variant="outline">
              Add Medicine
            </Button>
            <Button type="submit">Create Prescription</Button>
          </div>
          {error && <div className="p-4 text-red-500 bg-red-50 rounded-lg">{error}</div>}
        </form>
      </CardContent>
    </Card>
  )
}

