BEGIN TRANSACTION;
ALTER TABLE "VleratProdukteve" ADD "Bashkangjitje" TEXT NULL;

ALTER TABLE "VleratProdukteve" ADD "EmeriBashkangjitjes" TEXT NULL;

UPDATE "VleratProdukteve" SET "Bashkangjitje" = NULL, "EmeriBashkangjitjes" = NULL
WHERE "Id" = 1;
SELECT changes();


UPDATE "VleratProdukteve" SET "Bashkangjitje" = NULL, "EmeriBashkangjitjes" = NULL
WHERE "Id" = 2;
SELECT changes();


UPDATE "VleratProdukteve" SET "Bashkangjitje" = NULL, "EmeriBashkangjitjes" = NULL
WHERE "Id" = 3;
SELECT changes();


UPDATE "VleratProdukteve" SET "Bashkangjitje" = NULL, "EmeriBashkangjitjes" = NULL
WHERE "Id" = 4;
SELECT changes();


UPDATE "VleratProdukteve" SET "Bashkangjitje" = NULL, "EmeriBashkangjitjes" = NULL
WHERE "Id" = 5;
SELECT changes();


UPDATE "VleratProdukteve" SET "Bashkangjitje" = NULL, "EmeriBashkangjitjes" = NULL
WHERE "Id" = 6;
SELECT changes();


UPDATE "VleratProdukteve" SET "Bashkangjitje" = NULL, "EmeriBashkangjitjes" = NULL
WHERE "Id" = 7;
SELECT changes();


UPDATE "VleratProdukteve" SET "Bashkangjitje" = NULL, "EmeriBashkangjitjes" = NULL
WHERE "Id" = 8;
SELECT changes();


UPDATE "VleratProdukteve" SET "Bashkangjitje" = NULL, "EmeriBashkangjitjes" = NULL
WHERE "Id" = 9;
SELECT changes();


UPDATE "VleratProdukteve" SET "Bashkangjitje" = NULL, "EmeriBashkangjitjes" = NULL
WHERE "Id" = 10;
SELECT changes();


UPDATE "VleratProdukteve" SET "Bashkangjitje" = NULL, "EmeriBashkangjitjes" = NULL
WHERE "Id" = 11;
SELECT changes();


UPDATE "VleratProdukteve" SET "Bashkangjitje" = NULL, "EmeriBashkangjitjes" = NULL
WHERE "Id" = 12;
SELECT changes();


UPDATE "VleratProdukteve" SET "Bashkangjitje" = NULL, "EmeriBashkangjitjes" = NULL
WHERE "Id" = 13;
SELECT changes();


UPDATE "VleratProdukteve" SET "Bashkangjitje" = NULL, "EmeriBashkangjitjes" = NULL
WHERE "Id" = 14;
SELECT changes();


UPDATE "VleratProdukteve" SET "Bashkangjitje" = NULL, "EmeriBashkangjitjes" = NULL
WHERE "Id" = 15;
SELECT changes();


UPDATE "VleratProdukteve" SET "Bashkangjitje" = NULL, "EmeriBashkangjitjes" = NULL
WHERE "Id" = 16;
SELECT changes();


UPDATE "VleratProdukteve" SET "Bashkangjitje" = NULL, "EmeriBashkangjitjes" = NULL
WHERE "Id" = 17;
SELECT changes();


UPDATE "VleratProdukteve" SET "Bashkangjitje" = NULL, "EmeriBashkangjitjes" = NULL
WHERE "Id" = 18;
SELECT changes();


UPDATE "VleratProdukteve" SET "Bashkangjitje" = NULL, "EmeriBashkangjitjes" = NULL
WHERE "Id" = 19;
SELECT changes();


UPDATE "VleratProdukteve" SET "Bashkangjitje" = NULL, "EmeriBashkangjitjes" = NULL
WHERE "Id" = 20;
SELECT changes();


INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251125124531_AddAttachmentToVleraProduktit', '10.0.0');

COMMIT;

