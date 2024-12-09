import React, { useContext, useState } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import { useParams, useNavigate } from "react-router-dom";
import AuthContext from "./AuthContext";
import { addRating } from "../services/apiService";
import "bootstrap/dist/css/bootstrap.min.css";
import "./Bookmark.css";
import "./NavBar.css";
import StarRating from "./StarRating";

const Rate = ({ show, onClose }) => {
    const [rating, setRating] = useState("");
    const { isLoggedIn } = useContext(AuthContext);
    const { tConst } = useParams();
    const navigate = useNavigate();

    const handleRate = async () => {
        try {
            await addRating(tConst, rating);
            alert("Rating added successfully!");
            onClose();
        } catch (err) {
            console.error("Error adding rating:", err);
            alert("Failed to add rating.");
        }
    };

    const handleButtonClick = () => {
        if (isLoggedIn) {
            handleRate();
        } else {
            navigate("/login");
        }
    };

    return (
        <Modal show={show} onHide={onClose} className="bookmark-modal">
            <Modal.Header closeButton>
                <Modal.Title className="bookmark-title">Rate Title</Modal.Title>
            </Modal.Header>
            <Modal.Body className="mt-3">
                {!isLoggedIn ? (
                    <p className="bookmark-message">
                        You must be logged in to rate this title.
                    </p>
                ) : (
                   <Form>
                    <Form.Group controlId="rating">
                      <Form.Label>Select your rating (1 to 10)</Form.Label>
                      <StarRating rating={rating} setRating={setRating} />
                    </Form.Group>
                  </Form>
                )}
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={onClose}>
                    Cancel
                </Button>
                <Button
                    onClick={handleButtonClick}
                    className="navbar-button"
                >
                    {isLoggedIn ? "Save" : "Login"}
                </Button>
            </Modal.Footer>
        </Modal>
    );
};

export default Rate;
