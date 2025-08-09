using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class dbSetAddedBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_AspNetUsers_UserId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Destinations_TourId",
                table: "Booking");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Booking",
                table: "Booking");

            migrationBuilder.RenameTable(
                name: "Booking",
                newName: "Bookings");

            migrationBuilder.RenameIndex(
                name: "IX_Booking_UserId",
                table: "Bookings",
                newName: "IX_Bookings_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Booking_TourId",
                table: "Bookings",
                newName: "IX_Bookings_TourId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "df1c3a0f-1234-4cde-bb55-d5f15a6aabcd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2e2ae977-b2b8-4dd4-ba1e-88901622f1ac", "AQAAAAIAAYagAAAAEMWcLhLwkpJ8J4Mi9HclQ5c/uxBVMBRxJoy3dSG5PM+/fTH00xmUIwIPUkE12/bBDQ==", "a14bcea4-1737-4f38-881e-95633d0d43e5" });

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 9, 7, 43, 36, 970, DateTimeKind.Local).AddTicks(6855));

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 9, 7, 43, 36, 970, DateTimeKind.Local).AddTicks(6915));

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 9, 7, 43, 36, 970, DateTimeKind.Local).AddTicks(6918));

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_AspNetUsers_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Destinations_TourId",
                table: "Bookings",
                column: "TourId",
                principalTable: "Destinations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_AspNetUsers_UserId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Destinations_TourId",
                table: "Bookings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings");

            migrationBuilder.RenameTable(
                name: "Bookings",
                newName: "Booking");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_UserId",
                table: "Booking",
                newName: "IX_Booking_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_TourId",
                table: "Booking",
                newName: "IX_Booking_TourId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Booking",
                table: "Booking",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_AspNetUsers_UserId",
                table: "Booking",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Destinations_TourId",
                table: "Booking",
                column: "TourId",
                principalTable: "Destinations",
                principalColumn: "Id");
        }
    }
}
