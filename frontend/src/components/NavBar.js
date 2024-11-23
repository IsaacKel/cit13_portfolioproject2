import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Navbar,
  Nav,
  Form,
  FormControl,
  Button,
  Row,
  Col,
  Modal,
} from "react-bootstrap";
import "./NavBar.css";
import Login from "../pages/Login";
import SignUp from "../pages/SignUp";

const NavBar = () => {
  const [showLogin, setShowLogin] = useState(false);
  const [showSignUp, setShowSignUp] = useState(false);

  const handleCloseLogin = () => setShowLogin(false);
  const handleShowLogin = () => setShowLogin(true);

  const handleCloseSignUp = () => setShowSignUp(false);
  const handleShowSignUp = () => setShowSignUp(true);

  const [searchQuery, setSearchQuery] = useState("");
  const navigate = useNavigate();

  const handleSearch = () => {
    if (searchQuery.trim()) {
      navigate(`/search?query=${encodeURIComponent(searchQuery)}`);
    }
  };

  return (
    <Navbar bg="light" expand="lg" className="p-3">
      <Row className="w-100 align-items-center">
        {/* Column 1: Logo */}
        <Col xs={3} className="d-flex align-items-center">
          <Navbar.Brand href="/" className="pl-3">
            <img
              src={require("../images/tmdbLogo.svg").default}
              alt="Logo"
              style={{ height: "4rem" }}
            />
          </Navbar.Brand>
        </Col>
        {/* Column 2: Search bar and Nav Links */}
        <Col xs={6}>
          <Row className="align-items-center">
            {/* Search bar with Button */}
            <Col xs={12} className="d-flex justify-content-center">
              <Form inline className="d-flex w-75">
                <FormControl
                  type="text"
                  placeholder="Search"
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                  className="mr-2 flex-grow-1"
                />
                <Button variant="outline-success" onClick={handleSearch}>
                  Search
                </Button>
              </Form>
            </Col>
          </Row>
          <Row>
            {/* Navbar links */}
            <Nav className="w-100 justify-content-center navbar-links mt-2">
              <Nav.Link href="/" className="px-3">
                Home
              </Nav.Link>
              <Nav.Link href="/titles/movies" className="px-3">
                Movies
              </Nav.Link>
              <Nav.Link href="/titles/tvshows" className="px-3">
                TV Shows
              </Nav.Link>
              <Nav.Link href="/people" className="px-3">
                People
              </Nav.Link>
            </Nav>
          </Row>
        </Col>

        {/* Column 3: Login and Sign Up */}
        <Col xs={3} className="d-flex justify-content-end">
          <Button
            variant="outline-primary"
            className="mr-2"
            onClick={handleShowLogin}
          >
            Log In
          </Button>
          <Button variant="primary" className="ml-2" onClick={handleShowSignUp}>
            Sign Up
          </Button>
        </Col>
      </Row>

      <Modal show={showLogin} onHide={handleCloseLogin}>
        <Modal.Header closeButton>
          <Modal.Title>Log In</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Login />
        </Modal.Body>
      </Modal>

      <Modal show={showSignUp} onHide={handleCloseSignUp}>
        <Modal.Header closeButton>
          <Modal.Title>Sign Up</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <SignUp />
        </Modal.Body>
      </Modal>
    </Navbar>
  );
};

export default NavBar;
