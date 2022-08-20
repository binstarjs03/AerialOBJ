using System;
using binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;
using binstarjs03.MinecraftSharpOBJ.Nbt.Concrete;
namespace binstarjs03.MinecraftSharpOBJ;

internal class Program {
    static void Main(string[] args) {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Run2();
        Console.ReadKey();
    }

    static void Run2() {
        string path = @"C:\Users\Bin\repos\Bin Resource\ExampleTags\level.dat";
        //string path = @"C:\Users\Bin\repos\Bin Resource\ExampleTags\bigtest.nbt";
        NbtBase nbt = NbtBase.ReadDisk(path, Utils.IO.ByteOrder.BigEndian);
        Console.WriteLine(nbt.GetTree());
    }

    static void Run() {
        NbtList nbtList = new(
            "Vector3", new NbtBase[] {
                new NbtInt("X", 9),
                new NbtInt("Y", 3),
                new NbtInt("Z", 15),
            }
        );
        Console.WriteLine(nbtList.GetTree());

        NbtInt level = new("Level", 70);
        Console.WriteLine(level.GetTree());

        NbtCompound player = new("Player", new NbtBase[] {
            new NbtString("Name", "Steve"),
            level,
            new NbtFloat("Strength", 3.97f),
            new NbtCompound("Position", new NbtBase[] {
                new NbtInt("X", 9),
                new NbtInt("Y", 3),
                new NbtInt("Z", 15),
            }),
            nbtList,
        });
        Console.WriteLine(player.GetTree());

        NbtCompound person = new("Person", new NbtBase[] {
            new NbtString("Name", "Steve"),
            new NbtInt("Age", 26),
            new NbtString("Occupation", "Explorer"),
            new NbtList("Items", new NbtBase[] {
                new NbtCompound("Item Property", new NbtBase[] {
                    new NbtString("Name", "Diamond Sword"),
                    new NbtFloat("Durability", 78.3f),
                }),
                new NbtCompound("Item Property", new NbtBase[] {
                    new NbtString("Name", "Compass"),
                }),
                new NbtCompound("Item Property", new NbtBase[] {
                    new NbtString("Name", "Book and quill"),
                    new NbtString("Content", "Hello, this is my diary"),
                }),
            }),
            new NbtCompound("Equipped Armors", new NbtBase[] {
                new NbtCompound("Head"),
                new NbtCompound("Torso", new NbtBase[] {
                    new NbtCompound("Item Property", new NbtBase[] {
                        new NbtString("Name", "Linen TShirt"),
                        new NbtFloat("Durability", 100f),
                    }),
                    new NbtCompound("Enchantment"),
                }),
                new NbtCompound("Legging", new NbtBase[] {
                    new NbtCompound("Item Property", new NbtBase[] {
                        new NbtString("Name", "Linen Legging"),
                        new NbtFloat("Durability", 100f),
                    }),
                    new NbtCompound("Enchantment"),
                }),
                new NbtCompound("Boots", new NbtBase[] {
                    new NbtCompound("Item Property", new NbtBase[] {
                        new NbtString("Name", "Leather Boots"),
                        new NbtFloat("Durability", 85.28f),
                    }),
                    new NbtCompound("Enchantment"),
                }),
            }),
        });
        Console.WriteLine(person.GetTree());
    }
}
