using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using QuartzTesting.Models;

namespace QuartzTesting.Context
{
    public partial class ImportedToDoContext : DbContext
    {
        public ImportedToDoContext()
        {
        }

        public ImportedToDoContext(DbContextOptions<ImportedToDoContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ImportedTodo> ImportedTodos { get; set; } = null!;

    

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ImportedTodo>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name")
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
