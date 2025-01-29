"use client"

import { useEffect, useState } from "react"
import { useRouter } from "next/navigation"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"

export default function Dashboard() {
  const [userRole, setUserRole] = useState<string | null>(null)
  const router = useRouter()

  useEffect(() => {
    const token = localStorage.getItem("token")
    if (!token) {
      router.push("/")
    } else {
      // Decode the JWT token to get the user's role
      const payload = JSON.parse(atob(token.split(".")[1]))
      setUserRole(payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"])
    }
  }, [router])

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle>Welcome, {userRole}</CardTitle>
          <CardDescription>Manage prescriptions and patient information</CardDescription>
        </CardHeader>
        <CardContent>
          {userRole === "Doctor" && (
            <Button onClick={() => router.push("/dashboard/create-prescription")}>Create Prescription</Button>
          )}
          {userRole === "Pharmacy" && (
            <Button onClick={() => router.push("/dashboard/view-prescriptions")}>View Prescriptions</Button>
          )}
        </CardContent>
      </Card>
    </div>
  )
}

