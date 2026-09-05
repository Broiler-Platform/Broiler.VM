export let ready = "not-yet";

ready = await Promise.resolve("awaited");
print("dependency finished with " + ready);
