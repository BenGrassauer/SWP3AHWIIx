import { PrismaClient } from "./prisma/client/client.ts";
//import {}
const prisma = new PrismaClient();

const seed_users = [
  { name: "Ben", email: "ben@example.com", password: "abc123" },
  { name: "jerk", email: "jerk@example.com", password: "abc123" },
  { name: "marcel", email: "marcel@example.com", password: "abc123" },
  { name: "dejan", email: "dejan@example.com", password: "abc123" },
  { name: "Georg", email: "Georg@example.com", password: "abc123" },
];

const seed_posts = [
  { title: "Hello World", content: "guten Tag", username: "marcel" },
  { title: "Hello Mars", content: "its cold", username: "Ben" },
  { title: "Guten Morgen", content: "habe hunger", username: "jerk" },
  { title: "Guten Abend", content: "habe durst", username: "dejan" },
  { title: "Guten Nacht", content: "habe schlaf", username: "Georg" },
];

export async function seed() {
  for (const user of seed_users) {
    await prisma.user.create({
      data: user,
    });
  }
  for (const post of seed_posts) {
    await prisma.post.create({
      data: {
        title: post.title,
        content: post.content,
        user: { connect: { name: post.username } },
      },
    });
  }
}

if (import.meta.main) {
  await seed();
  console.log("Seeding finished.");
}
