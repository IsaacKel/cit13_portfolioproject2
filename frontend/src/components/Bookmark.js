import React, { useContext, useState } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import { useParams } from "react-router-dom";
import { addBookmark } from "../services/apiService";
import AuthContext from "./AuthContext";
import "bootstrap/dist/css/bootstrap.min.css";

const Bookmark = ({ show, onClose }) => {
  const [note, setNote] = useState("");
  const isLoggedIn = useContext(AuthContext);

  // Get tConst from URL parameters
  const { tConst } = useParams();
  if (!tConst) {
    console.error("tConst is missing from the URL parameters.");
  }
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
    <Modal show={show} onHide={onClose}>
      <Modal.Header closeButton>
        <Modal.Title>Add to Bookmarks</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        {!isLoggedIn ? (
          <p>You must be logged in to add a bookmark.</p>
        ) : (
          <Form>
            <Form.Group controlId="note">
              <Form.Label>Note:</Form.Label>
              <Form.Control
                type="text"
                value={note}
                onChange={(e) => setNote(e.target.value)}
              />
            </Form.Group>
          </Form>
        )}
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={onClose}>
          Cancel
        </Button>
        <Button
          variant="primary"
          onClick={handleBookmark}
          disabled={!isLoggedIn}
        >
          Save
        </Button>
      </Modal.Footer>
    </Modal>
  );
};

export default Bookmark;
