import React, { useState } from "react";
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

const NavBar = () => {
  const [show, setShow] = useState(false);

  const handleClose = () => setShow(false);
  const handleShow = () => setShow(true);

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
                  className="mr-2 flex-grow-1"
                />
                <Button variant="outline-success">Search</Button>
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
            onClick={handleShow}
          >
            Log In
          </Button>
          <Button variant="primary" className="ml-2">
            Sign Up
          </Button>
        </Col>
      </Row>
      <Modal show={show} onHide={handleClose}>
        <Modal.Header closeButton>
          <Modal.Title>Log In</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Login />
        </Modal.Body>
      </Modal>
    </Navbar>
  );
};

export default NavBar;
