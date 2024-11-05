-- Insert test data into userRatings table
INSERT INTO userRatings (userratingid, userId, tconst, rating, ratingDate) OVERRIDING SYSTEM VALUE VALUES
(1, 1, 'tt28638980', 8, NOW() AT TIME ZONE 'UTC'),
(2, 2, 'tt28638980', 7, NOW() AT TIME ZONE 'UTC'),
(3, 1, 'tt28638980', 10, NOW() AT TIME ZONE 'UTC');

-- Insert test data into userSearchHistory table
INSERT INTO usersearchHistory (searchid, userId, searchQuery, searchDate) OVERRIDING SYSTEM VALUE VALUES
(1, 1, 'Action movies', NOW() AT TIME ZONE 'UTC'),
(2, 2, 'Comedy films', NOW() AT TIME ZONE 'UTC'),
(3, 1, 'Drama series', NOW() AT TIME ZONE 'UTC');

-- Insert test data into userBookmarks table
INSERT INTO userBookmarks (bookmarkid, userId, tconst, nconst, note, bookmarkDate) OVERRIDING SYSTEM VALUE VALUES
(1, 1, 'tt28638980', null, 'Great movie to watch!', NOW() AT TIME ZONE 'UTC'),
(2, 2, 'tt28638980', null, 'Recommended for friends.', NOW() AT TIME ZONE 'UTC'),
(3, 3, null, 'nm0000703', 'Loved the storyline!', NOW() AT TIME ZONE 'UTC');

INSERT INTO users (userid, username, email, password, salt, name, role) OVERRIDING SYSTEM VALUE 
VALUES 
(1, 'user1', 'user1@example.com', 'hashedpassword1', 'randomsalt1', 'User One', 'Admin'),
(2, 'user2', 'user2@example.com', 'hashedpassword2', 'randomsalt2', 'User Two', 'User'),
(3, 'user3', 'user3@example.com', 'hashedpassword3', 'randomsalt3', 'User Three', 'Moderator'); 
