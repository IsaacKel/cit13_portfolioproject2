import React, { useContext, useState } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import { useParams } from "react-router-dom";
import { addBookmark } from "../services/apiService";
import AuthContext from "./AuthContext";
import "bootstrap/dist/css/bootstrap.min.css";
import "./Bookmark.css";
import "./NavBar.css";

const Bookmark = ({ show, onClose }) => {
  const [note, setNote] = useState("");
  const { isLoggedIn } = useContext(AuthContext);

  const { tConst } = useParams();

  const handleBookmark = async () => {
    if (!isLoggedIn) {
      alert("You need to be logged in to bookmark titles.");
      return;
    }

    try {
      await addBookmark(tConst, note);
      alert("Bookmark added successfully!");
      onClose();
    } catch (err) {
      console.error("Error adding bookmark:", err);
      alert("Failed to add bookmark.");
    }
  };

  return (
    <Modal show={show} onHide={onClose} className="bookmark-modal">
      <Modal.Header closeButton>
        <Modal.Title className="bookmark-title">Add Bookmark</Modal.Title>
      </Modal.Header>
      <Modal.Body className="mt-3">
        {!isLoggedIn ? (
          <p className="bookmark-message">
            You must be logged in to add a bookmark.
          </p>
        ) : (
          <Form>
            <Form.Group controlId="note">
              <Form.Control
                as="textarea"
                rows={4}
                value={note}
                onChange={(e) => setNote(e.target.value)}
                className="bookmark-input"
                placeholder="Write something..."
              />
            </Form.Group>
          </Form>
        )}
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={onClose}>
          Cancel
        </Button>
        <button
          onClick={handleBookmark}
          disabled={!isLoggedIn}
          className="navbar-button"
        >
          Save
        </button>
      </Modal.Footer>
    </Modal>
  );
};

export default Bookmark;
