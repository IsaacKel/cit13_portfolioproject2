CREATE TABLE users (
   userId INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
   username VARCHAR(50),
   email VARCHAR(100),
   password VARCHAR(100)
);

CREATE TABLE userRatings (
    userRatingId INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY, 
    userId INT,
    tconst VARCHAR(10),
    rating DECIMAL(3, 1),
    ratingDate DATE,
    FOREIGN KEY (userId) REFERENCES users(userId) ON DELETE CASCADE,
    FOREIGN KEY (tconst) REFERENCES titleBasic(tconst) ON DELETE CASCADE
);

CREATE TABLE userSearchHistory (
    searchId INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    userId INT,
    searchQuery TEXT,
    searchDate DATE,
    FOREIGN KEY (userId) REFERENCES users(userId) ON DELETE CASCADE
);

CREATE TABLE userBookmarks (
    bookmarkId INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    userId INT,
    tconst VARCHAR(20),
    nconst VARCHAR(20),
    note TEXT,
    bookmarkDate DATE,
    FOREIGN KEY (userId) REFERENCES users(userId) ON DELETE CASCADE,
    FOREIGN KEY (tconst) REFERENCES titleBasic(tconst) ON DELETE CASCADE,
    FOREIGN KEY (nconst) REFERENCES nameBasic(nconst) ON DELETE CASCADE
);

ALTER TABLE userRatings
ALTER COLUMN rating TYPE integer USING rating::integer; 