import sqlite3
import os

# Define the database file name
db_filename = "payments.db"

# Remove existing file if exists for a fresh start (optional)
if os.path.exists(db_filename):
    os.remove(db_filename)

# Connect to SQLite database (this creates the file if not exists)
conn = sqlite3.connect(db_filename)
c = conn.cursor()

# Create table Payments
c.execute("""
CREATE TABLE IF NOT EXISTS Payments (
    PaymentId INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER,
    Amount DECIMAL,
    Date TEXT
)
""")

# Clear any existing data (shouldn't be any since file is new)
c.execute("DELETE FROM Payments")

# Insert sample data
# Using fixed dates based on the sample in the C# code, relative to 2025-03-31
# 10 days ago: 2025-03-21, 5 days ago: 2025-03-26
c.execute("INSERT INTO Payments (UserId, Amount, Date) VALUES (?, ?, ?)", (1, 100.0, "2025-03-21T00:00:00Z"))
c.execute("INSERT INTO Payments (UserId, Amount, Date) VALUES (?, ?, ?)", (1, 150.0, "2025-03-26T00:00:00Z"))

conn.commit()
conn.close()

# Verify file creation and file size for debugging
file_info = os.stat(db_filename)
print(f"Created {db_filename} ({file_info.st_size} bytes)")
