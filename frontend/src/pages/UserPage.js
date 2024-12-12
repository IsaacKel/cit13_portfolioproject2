import React, { useEffect, useState, useContext } from "react";
import {
    fetchUserData,
    fetchBookmarks,
    fetchUserRatings,
    fetchUserSearchHistory,
    fetchTitleData,
    fetchNameData,
    fetchImages,
    deleteRating,
    deleteBookmark,
    deleteSearchHistory,
} from "../services/apiService";
import AuthContext from "../components/AuthContext";
import Login from "../pages/Login";
import { Modal } from "react-bootstrap";

const UserPage = () => {
    const { isLoggedIn } = useContext(AuthContext);
    const [showLoginModal, setShowLoginModal] = useState(false);
    const [user, setUser] = useState(null);
    const [bookmarks, setBookmarks] = useState([]);
    const [ratings, setRatings] = useState([]);
    const [searchHistory, setSearchHistory] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    // Trigger login modal if the user is not logged in
    useEffect(() => {
        if (!isLoggedIn) {
            setShowLoginModal(true);
        }
    }, [isLoggedIn]);

    useEffect(() => {
        if (!isLoggedIn) return;

        const fetchData = async () => {
            try {
                const [userData, userBookmarks, userRatings, userSearchHistory] =
                    await Promise.all([
                        fetchUserData(),
                        fetchBookmarks(),
                        fetchUserRatings(),
                        fetchUserSearchHistory(),
                    ]);

                setUser(userData || {});

                const bookmarksWithTitles = await Promise.all(
                    userBookmarks.items.map(async (bookmark) => {
                        if (bookmark.tConst) {
                            const titleData = await fetchTitleData(bookmark.tConst);
                            return { ...bookmark, title: titleData };
                        } else if (bookmark.nConst) {
                            const nameData = await fetchNameData(bookmark.nConst);
                            const nameImage = await fetchImages(bookmark.nConst);
                            return { ...bookmark, name: nameData, image: nameImage };
                        }
                        return bookmark;
                    })
                );
                setBookmarks({ items: bookmarksWithTitles });
            } catch (err) {
                console.error(err);
            }

            try {
                const userRatings = await fetchUserRatings();
                const ratingsWithTitles = await Promise.all(
                    userRatings.items.map(async (rating) => {
                        const titleData = await fetchTitleData(rating.tConst);
                        return { ...rating, title: titleData };
                    })
                );
                setRatings(ratingsWithTitles || []);
            } catch (err) {
                setRatings([]);
            }

            try {
                const userSearchHistory = await fetchUserSearchHistory();
                setSearchHistory(userSearchHistory || []);
            } catch (err) {
                setSearchHistory([]);
            }

            setLoading(false);
        };

        fetchData();
    }, [isLoggedIn]);

    const handleDeleteRating = async (userId, ratingId) => {
        try {
            await deleteRating(userId, ratingId);
            setRatings((prevRatings) => prevRatings.filter((r) => r.id !== ratingId));
        } catch (err) {
            setError(err.message);
        }
    };
    const handleDeleteBookmark = async (bookmarkId) => {
        try {
            await deleteBookmark(bookmarkId);
            setBookmarks((prevBookmarks) => ({
                items: prevBookmarks.items.filter((b) => b.id !== bookmarkId),
            }));
        } catch (err) {
            setError(err.message);
        }
    };
    const handleDeleteSearchHistory = async (searchHistoryId) => {
        try {
            await deleteSearchHistory(searchHistoryId);
            setSearchHistory((prevSearchHistory) => ({
                items: prevSearchHistory.items.filter((s) => s.id !== searchHistoryId),
            }));
        } catch (err) {
            setError(err.message);
        }
    };

    if (loading) return <p>Loading...</p>;
    if (error) return <p>Error: {error}</p>;

    return (
        <div style={{ color: "white" }}>
            <h1>User Page</h1>
            {isLoggedIn ? (
                <>
                    {user && (
                        <div>
                            <p>
                                <strong>ID:</strong> {user.id}
                            </p>
                            <p>
                                <strong>Email:</strong> {user.email}
                            </p>
                            <p>
                                <strong>Username:</strong> {user.username}
                            </p>
                        </div>
                    )}
                    {/* Additional content for bookmarks, ratings, etc. */}
                </>
            ) : (
                <Modal show={showLoginModal} onHide={() => setShowLoginModal(false)}>
                    <Modal.Header closeButton>
                        <Modal.Title>Log In</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Login onLoginSuccess={() => setShowLoginModal(false)} />
                    </Modal.Body>
                </Modal>
            )}
        </div>
    );
};

export default UserPage;
