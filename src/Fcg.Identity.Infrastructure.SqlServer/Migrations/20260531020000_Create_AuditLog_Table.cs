using System;
using System.Diagnostics.CodeAnalysis;
using Fcg.Identity.Infrastructure.SqlServer.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fcg.Identity.Infrastructure.SqlServer.Migrations;

/// <inheritdoc />
[ExcludeFromCodeCoverage]
[DbContext(typeof(FcgIdentityDbContext))]
[Migration("20260531020000_Create_AuditLog_Table")]
public partial class Create_AuditLog_Table : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AuditLogs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ActorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                ActorType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                EntityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CorrelationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AuditLogs", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_AuditLogs_Action",
            table: "AuditLogs",
            column: "Action");

        migrationBuilder.CreateIndex(
            name: "IX_AuditLogs_CorrelationId",
            table: "AuditLogs",
            column: "CorrelationId");

        migrationBuilder.CreateIndex(
            name: "IX_AuditLogs_EntityName_EntityId",
            table: "AuditLogs",
            columns: ["EntityName", "EntityId"]);

        migrationBuilder.CreateIndex(
            name: "IX_AuditLogs_OccurredAt",
            table: "AuditLogs",
            column: "OccurredAt");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AuditLogs");
    }
}
