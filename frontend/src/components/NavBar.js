import React, { useState, useEffect } from "react";
import { Navbar, Nav, Button, Row, Col, Modal } from "react-bootstrap";
import "./NavBar.css";
import Login from "../pages/Login";
import SignUp from "../pages/SignUp";
import { logoutUser } from "../services/apiService";
import SearchBar from "./SearchBar";

const NavBar = () => {
  const [isLoggedIn, setIsLoggedIn] = useState(null);
  const [showLogin, setShowLogin] = useState(false);
  const [showSignUp, setShowSignUp] = useState(false);
  const [showLogout, setShowLogout] = useState(false);

  const handleCloseLogin = () => setShowLogin(false);
  const handleShowLogin = () => setShowLogin(true);

  const handleCloseSignUp = () => setShowSignUp(false);
  const handleShowSignUp = () => setShowSignUp(true);

  const handleCloseLogout = () => setShowLogout(false);
  const handleShowLogout = () => setShowLogout(true);

  const handleLogout = async () => {
  try {
      await logoutUser();
    setIsLoggedIn(false);
    handleCloseLogout();
  } catch (error) {
    console.error("Error logging out:", error);
  }
};

    useEffect(() => {
        const cookieTokenExists = document.cookie.split("; ").some((cookie) => cookie.startsWith("auth_token_cookie="));
        const localStorageTokenExists = Boolean(localStorage.getItem("token"));

        setIsLoggedIn(cookieTokenExists || localStorageTokenExists);
    }, []);

    useEffect(() => {
        console.log("Is the user logged In? :", isLoggedIn);
    }, [isLoggedIn]);

  return (
    <Navbar bg="dark" expand="lg" className="p-3">
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
              <SearchBar />
            </Col>
          </Row>
        </Col>

        {/* Column 3: Login and Sign Up and Log Out*/}
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
          <Button
            variant="outline-danger"
            className="ml-2"
            onClick={handleShowLogout}
           >
           Log Out
          </Button>
        </Col>
      </Row>

      <Modal show={showLogin} onHide={handleCloseLogin}>
        <Modal.Header closeButton>
          <Modal.Title>Log In</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Login onLoginSuccess={handleCloseLogin} /> 
        </Modal.Body>
      </Modal>

      <Modal show={showSignUp} onHide={handleCloseSignUp}>
        <Modal.Header closeButton>
          <Modal.Title>Sign Up</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <SignUp onSignupSuccess={handleCloseSignUp} />
        </Modal.Body>
      </Modal>
        <Modal show={showLogout} onHide={handleCloseLogout}>
            <Modal.Header closeButton>
                <Modal.Title>Are you sure you want to log out?</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Button
                    variant="danger"
                    className="mr-3"
                    onClick={handleLogout}
                >
                    Yes, Log Out
                </Button>
                <Button variant="secondary" onClick={handleCloseLogout}>
                    Cancel
                </Button>
            </Modal.Body>
        </Modal>
      </Navbar>
  );
};

export default NavBar;
