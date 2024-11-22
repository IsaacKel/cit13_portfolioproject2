import React from "react";
import {
  Navbar,
  Nav,
  Form,
  FormControl,
  Button,
  Row,
  Col,
} from "react-bootstrap";
import "./NavBar.css";

const NavBar = () => {
  return (
    <Navbar bg="light" expand="lg" className="p-3">
      <Row className="w-100 align-items-center">
        {/* Column 1: Logo */}
        <Col xs={3} className="d-flex align-items-center">
          <Navbar.Brand href="/" className="pl-3">
            <img
              src="/path/to/logo.png"
              alt="Logo"
              style={{ height: "50px" }} // Adjust the logo size as needed
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
              <Nav.Link href="/titles/people" className="px-3">
                People
              </Nav.Link>
            </Nav>
          </Row>
        </Col>

        {/* Column 3: Login and Sign Up */}
        <Col xs={3} className="d-flex justify-content-end">
          <Button variant="outline-primary" className="mr-2">
            Log In
          </Button>
          <Button variant="primary" className="ml-2">
            Sign Up
          </Button>
        </Col>
      </Row>
    </Navbar>
  );
};

export default NavBar;
