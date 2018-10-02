using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using WorkflowCore.Persistence.EntityFramework;

namespace WorkflowCore.Persistence.PostgreSQL.Migrations
{
    public partial class ControlStructures : Migration
    {
        private readonly string _tablePrefix = MigrationMetaInfo.TableNamePrefix;
        private readonly string _schema = MigrationMetaInfo.DbSchema;
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExecutionError_ExecutionPointer_ExecutionPointerId",
                schema: _schema,
                table: _tablePrefix + "ExecutionError");

            migrationBuilder.DropIndex(
                name: "IX_ExecutionError_ExecutionPointerId",
                schema: _schema,
                table: _tablePrefix + "ExecutionError");

            migrationBuilder.DropColumn(
                name: "PathTerminator",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: _schema,
                table: _tablePrefix + "ExecutionError");

            migrationBuilder.RenameColumn(
                name: "ConcurrentFork",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer",
                newName: "RetryCount");

            migrationBuilder.AlterColumn<string>(
                name: "StepName",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EventName",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EventKey",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Children",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContextItem",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PredecessorId",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExecutionPointerId",
                schema: _schema,
                table: _tablePrefix + "ExecutionError",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<string>(
                name: "WorkflowId",
                schema: _schema,
                table: _tablePrefix + "ExecutionError",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Children",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer");

            migrationBuilder.DropColumn(
                name: "ContextItem",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer");

            migrationBuilder.DropColumn(
                name: "PredecessorId",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer");

            migrationBuilder.DropColumn(
                name: "WorkflowId",
                schema: _schema,
                table: _tablePrefix + "ExecutionError");

            migrationBuilder.RenameColumn(
                name: "RetryCount",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer",
                newName: "ConcurrentFork");

            migrationBuilder.AlterColumn<string>(
                name: "StepName",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EventName",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EventKey",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PathTerminator",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<long>(
                name: "ExecutionPointerId",
                schema: _schema,
                table: _tablePrefix + "ExecutionError",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                schema: _schema,
                table: _tablePrefix + "ExecutionError",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionError_ExecutionPointerId",
                schema: _schema,
                table: _tablePrefix + "ExecutionError",
                column: "ExecutionPointerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExecutionError_ExecutionPointer_ExecutionPointerId",
                schema: _schema,
                table: _tablePrefix + "ExecutionError",
                column: "ExecutionPointerId",
                principalSchema: _schema,
                principalTable: _tablePrefix + "ExecutionPointer",
                principalColumn: "PersistenceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
