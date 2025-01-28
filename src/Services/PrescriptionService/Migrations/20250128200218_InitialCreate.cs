using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrescriptionService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Prescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientTc = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrescriptionItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineBarcode = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    MedicineName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Usage = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsSubmitted = table.Column<bool>(type: "bit", nullable: false),
                    PrescriptionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrescriptionItems_Prescriptions_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrescriptionSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PharmacyId = table.Column<int>(type: "int", nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrescriptionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrescriptionSubmissions_Prescriptions_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubmittedMedicineBarcode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Barcode = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    PrescriptionSubmissionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmittedMedicineBarcode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmittedMedicineBarcode_PrescriptionSubmissions_PrescriptionSubmissionId",
                        column: x => x.PrescriptionSubmissionId,
                        principalTable: "PrescriptionSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionItems_PrescriptionId",
                table: "PrescriptionItems",
                column: "PrescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionSubmissions_PrescriptionId",
                table: "PrescriptionSubmissions",
                column: "PrescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmittedMedicineBarcode_PrescriptionSubmissionId",
                table: "SubmittedMedicineBarcode",
                column: "PrescriptionSubmissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrescriptionItems");

            migrationBuilder.DropTable(
                name: "SubmittedMedicineBarcode");

            migrationBuilder.DropTable(
                name: "PrescriptionSubmissions");

            migrationBuilder.DropTable(
                name: "Prescriptions");
        }
    }
}
