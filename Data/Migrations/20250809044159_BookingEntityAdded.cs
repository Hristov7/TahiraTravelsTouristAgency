using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class BookingEntityAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TourId = table.Column<int>(type: "int", nullable: false),
                    NumberOfPeople = table.Column<int>(type: "int", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Booking_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Booking_Destinations_TourId",
                        column: x => x.TourId,
                        principalTable: "Destinations",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "df1c3a0f-1234-4cde-bb55-d5f15a6aabcd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "494e7676-a4fc-4a6d-b350-62a6adb1d055", "AQAAAAIAAYagAAAAEJwYwuI6A2mR99bmgENwmSfwDcZxUBBGTPz0T3w1qPZPqbmD6ZWxeoru+NMA+44egw==", "11e73e65-5632-424b-a55e-42253ecb4406" });

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 9, 7, 41, 58, 676, DateTimeKind.Local).AddTicks(7645));

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 9, 7, 41, 58, 676, DateTimeKind.Local).AddTicks(7690));

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 9, 7, 41, 58, 676, DateTimeKind.Local).AddTicks(7693));

            migrationBuilder.CreateIndex(
                name: "IX_Booking_TourId",
                table: "Booking",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_UserId",
                table: "Booking",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "df1c3a0f-1234-4cde-bb55-d5f15a6aabcd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e6a8b8fb-4590-4dbc-95db-dbf6fda4e50d", "AQAAAAIAAYagAAAAEEsOhA+73V7gutszXq6pV6ZPeqy3iYHwJtNM3c8cQtP9vkGTc5olJspGk+NKDrZTEA==", "aa2fa268-af80-48dd-989b-c1cb2c493132" });

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 7, 23, 58, 27, 73, DateTimeKind.Local).AddTicks(6156));

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 7, 23, 58, 27, 73, DateTimeKind.Local).AddTicks(6218));

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 7, 23, 58, 27, 73, DateTimeKind.Local).AddTicks(6221));
        }
    }
}
