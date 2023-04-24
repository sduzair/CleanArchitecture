using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreatedAdminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Value", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("43047ad3-9f46-4c46-b7a0-7a23015c5a80"), null, "Visitor", "VISITOR" },
                    { new Guid("7e2187ae-77a5-4e2a-94bb-49f517882178"), null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Value",
                keyValue: new Guid("43047ad3-9f46-4c46-b7a0-7a23015c5a80"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Value",
                keyValue: new Guid("7e2187ae-77a5-4e2a-94bb-49f517882178"));
        }
    }
}
