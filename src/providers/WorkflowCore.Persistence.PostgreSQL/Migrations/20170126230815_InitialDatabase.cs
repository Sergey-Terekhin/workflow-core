using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WorkflowCore.Persistence.PostgreSQL.Migrations
{
    public partial class InitialDatabase : Migration
    {
        private readonly string _schema = MigrationMetaInfo.DbSchema;
        private readonly string _tablePrefix = MigrationMetaInfo.TableNamePrefix;
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: _schema);

            migrationBuilder.CreateTable(
                name: _tablePrefix + "UnpublishedEvent",
                schema: _schema,
                columns: table => new
                {
                    PersistenceId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    EventData = table.Column<string>(nullable: true),
                    EventKey = table.Column<string>(maxLength: 200, nullable: true),
                    EventName = table.Column<string>(maxLength: 200, nullable: true),
                    PublicationId = table.Column<Guid>(nullable: false),
                    StepId = table.Column<int>(nullable: false),
                    WorkflowId = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnpublishedEvent", x => x.PersistenceId);
                });

            migrationBuilder.CreateTable(
                name: _tablePrefix + "Subscription",
                schema: _schema,
                columns: table => new
                {
                    PersistenceId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    EventKey = table.Column<string>(maxLength: 200, nullable: true),
                    EventName = table.Column<string>(maxLength: 200, nullable: true),
                    StepId = table.Column<int>(nullable: false),
                    SubscriptionId = table.Column<Guid>(maxLength: 200, nullable: false),
                    WorkflowId = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => x.PersistenceId);
                });

            migrationBuilder.CreateTable(
                name: _tablePrefix + "Workflow",
                schema: _schema,
                columns: table => new
                {
                    PersistenceId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CompleteTime = table.Column<DateTime>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    InstanceId = table.Column<Guid>(maxLength: 200, nullable: false),
                    NextExecution = table.Column<long>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    WorkflowDefinitionId = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workflow", x => x.PersistenceId);
                });

            migrationBuilder.CreateTable(
                name: _tablePrefix + "ExecutionPointer",
                schema: _schema,
                columns: table => new
                {
                    PersistenceId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Active = table.Column<bool>(nullable: false),
                    ConcurrentFork = table.Column<int>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: true),
                    EventData = table.Column<string>(nullable: true),
                    EventKey = table.Column<string>(nullable: true),
                    EventName = table.Column<string>(nullable: true),
                    EventPublished = table.Column<bool>(nullable: false),
                    Id = table.Column<string>(maxLength: 50, nullable: true),
                    PathTerminator = table.Column<bool>(nullable: false),
                    PersistenceData = table.Column<string>(nullable: true),
                    SleepUntil = table.Column<DateTime>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: true),
                    StepId = table.Column<int>(nullable: false),
                    StepName = table.Column<string>(nullable: true),
                    WorkflowId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionPointer", x => x.PersistenceId);
                    table.ForeignKey(
                        name: "FK_ExecutionPointer_Workflow_WorkflowId",
                        column: x => x.WorkflowId,
                        principalSchema: _schema,
                        principalTable: _tablePrefix + "Workflow",
                        principalColumn: "PersistenceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: _tablePrefix + "ExecutionError",
                schema: _schema,
                columns: table => new
                {
                    PersistenceId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ErrorTime = table.Column<DateTime>(nullable: false),
                    ExecutionPointerId = table.Column<long>(nullable: false),
                    Id = table.Column<string>(maxLength: 50, nullable: true),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionError", x => x.PersistenceId);
                    table.ForeignKey(
                        name: "FK_ExecutionError_ExecutionPointer_ExecutionPointerId",
                        column: x => x.ExecutionPointerId,
                        principalSchema: _schema,
                        principalTable: _tablePrefix + "ExecutionPointer",
                        principalColumn: "PersistenceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: _tablePrefix + "ExtensionAttribute",
                schema: _schema,
                columns: table => new
                {
                    PersistenceId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AttributeKey = table.Column<string>(maxLength: 100, nullable: true),
                    AttributeValue = table.Column<string>(nullable: true),
                    ExecutionPointerId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtensionAttribute", x => x.PersistenceId);
                    table.ForeignKey(
                        name: "FK_ExtensionAttribute_ExecutionPointer_ExecutionPointerId",
                        column: x => x.ExecutionPointerId,
                        principalSchema: _schema,
                        principalTable: _tablePrefix + "ExecutionPointer",
                        principalColumn: "PersistenceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionError_ExecutionPointerId",
                schema: _schema,
                table: _tablePrefix + "ExecutionError",
                column: "ExecutionPointerId");

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionPointer_WorkflowId",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtensionAttribute_ExecutionPointerId",
                schema: _schema,
                table: _tablePrefix + "ExtensionAttribute",
                column: "ExecutionPointerId");

            migrationBuilder.CreateIndex(
                name: "IX_UnpublishedEvent_PublicationId",
                schema: _schema,
                table: _tablePrefix + "UnpublishedEvent",
                column: "PublicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_EventKey",
                schema: _schema,
                table: _tablePrefix + "Subscription",
                column: "EventKey");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_EventName",
                schema: _schema,
                table: _tablePrefix + "Subscription",
                column: "EventName");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_SubscriptionId",
                schema: _schema,
                table: _tablePrefix + "Subscription",
                column: "SubscriptionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workflow_InstanceId",
                schema: _schema,
                table: _tablePrefix + "Workflow",
                column: "InstanceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workflow_NextExecution",
                schema: _schema,
                table: _tablePrefix + "Workflow",
                column: "NextExecution");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: _tablePrefix + "ExecutionError",
                schema: _schema);

            migrationBuilder.DropTable(
                name: _tablePrefix + "ExtensionAttribute",
                schema: _schema);

            migrationBuilder.DropTable(
                name: _tablePrefix + "UnpublishedEvent",
                schema: _schema);

            migrationBuilder.DropTable(
                name: _tablePrefix + "Subscription",
                schema: _schema);

            migrationBuilder.DropTable(
                name: _tablePrefix + "ExecutionPointer",
                schema: _schema);

            migrationBuilder.DropTable(
                name: _tablePrefix + "Workflow",
                schema: _schema);
        }
    }
}
