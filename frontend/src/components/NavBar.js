import React, { useState, useEffect, useContext } from "react";
import { Navbar, Button, Row, Col, Modal } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import "./NavBar.css";
import Login from "../pages/Login";
import SignUp from "../pages/SignUp";
import { logoutUser } from "../services/apiService";
import SearchBar from "./SearchBar";
import AuthContext from "./AuthContext";
const NavBar = () => {
  const { isLoggedIn, logout } = useContext(AuthContext);
  const [showLogin, setShowLogin] = useState(false);
  const [showSignUp, setShowSignUp] = useState(false);
  const [showLogout, setShowLogout] = useState(false);
  const navigate = useNavigate();

  const handleCloseLogin = () => setShowLogin(false);
  const handleShowLogin = () => setShowLogin(true);

  const handleCloseSignUp = () => setShowSignUp(false);
  const handleShowSignUp = () => setShowSignUp(true);

  const handleCloseLogout = () => setShowLogout(false);
  const handleShowLogout = () => setShowLogout(true);

  const handleLogout = async () => {
    try {
      await logoutUser();
      // After a successful server-side logout call the context's logout to update state and clear tokens
      logout();

      // Notify other tabs
      localStorage.setItem("logout", Date.now());
      localStorage.removeItem("token");
      handleCloseLogout();
    } catch (error) {
      console.error("Error logging out:", error);
    }
  };

  // Synchronize logout across tabs
  useEffect(() => {
    const syncLogout = (event) => {
      if (event.key === "logout") {
        logout();
        handleCloseLogin();
        handleCloseSignUp();
        handleCloseLogout();
      }
    };

    window.addEventListener("storage", syncLogout);
    return () => {
      window.removeEventListener("storage", syncLogout);
    };
  }, [logout]);

  useEffect(() => {
    console.log("Is the user logged In? :", isLoggedIn);
  }, [isLoggedIn]);

  const handleUserPage = () => {
    navigate("/user");
  };

  return (
    <Navbar expand="lg" className="p-3 navbar-dark">
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

        {/* Column 2: Search bar */}
        <Col xs={6}>
          <Row className="align-items-center">
            <Col xs={12} className="d-flex justify-content-center">
              <SearchBar />
            </Col>
          </Row>
        </Col>

        {/* Column 3: Auth Buttons */}
        <Col xs={3} className="d-flex justify-content-end">
          {!isLoggedIn ? (
            <>
              <Button
                variant="outline-light"
                className="mr-2"
                onClick={handleShowLogin}
              >
                Log In
              </Button>
              <Button
                variant="light"
                className="ml-2"
                onClick={handleShowSignUp}
              >
                Sign Up
              </Button>
            </>
          ) : (
            <>
              <Button
                variant="outline-info"
                className="ml-2"
                onClick={handleUserPage}
              >
                Profile
              </Button>
              <Button
                variant="outline-danger"
                className="ml-2"
                onClick={handleShowLogout}
              >
                Log Out
              </Button>
            </>
          )}
        </Col>
      </Row>

      {/* Modals */}
      <Modal show={showLogin} onHide={handleCloseLogin} className="dark-modal">
        <Modal.Header closeButton>
          <Modal.Title>Log In</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Login onLoginSuccess={handleCloseLogin} />
        </Modal.Body>
      </Modal>

      <Modal
        show={showSignUp}
        onHide={handleCloseSignUp}
        className="dark-modal"
      >
        <Modal.Header closeButton>
          <Modal.Title>Sign Up</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <SignUp onSignupSuccess={handleCloseSignUp} />
        </Modal.Body>
      </Modal>

      <Modal
        show={showLogout}
        onHide={handleCloseLogout}
        className="dark-modal"
      >
        <Modal.Header closeButton>
          <Modal.Title>Are you sure you want to log out?</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Button variant="danger" className="mr-3" onClick={handleLogout}>
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
