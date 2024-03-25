using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FpConsole;

public partial class DemoContext : DbContext
{
    public DemoContext()
    {
    }

    public DemoContext(DbContextOptions<DemoContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("server=192.168.29.82;user id=postgres;password=cq490910;database=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
        modelBuilder.Entity<Book>().HasKey(b => b.Id);
        modelBuilder.Entity<Book>().Property(b => b.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Book>().ComplexProperty(b => b.Price).IsRequired();
        modelBuilder.Entity<Book>().ComplexProperty(b => b.Aurthor, b => b.IsRequired());
        modelBuilder.Entity<Book>().OwnsMany(b => b.CoAuthors, cb => {
            cb.ToJson();
            cb.Property(x => x.Name).IsRequired();
            cb.Property(x => x.Email);
        });

    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
