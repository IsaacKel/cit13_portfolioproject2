import React from "react";
import { Form, Button } from "react-bootstrap";
import "./Login.css";

const SignUp = () => {
  return (
    <>
      <Form className="login-form">
        <Form.Label className="login-label">Username:</Form.Label>
        <Form.Control
          type="username"
          placeholder="Enter username"
          className="login-input"
        />
        <Form.Label className="login-label">Email:</Form.Label>
        <Form.Control
          type="username"
          placeholder="Enter email"
          className="login-input"
        />
        <Form.Label className="login-label">Password: </Form.Label>
        <Form.Control
          type="password"
          placeholder="Enter password"
          className="login-input"
        />
        <Form.Label className="login-label">Confirm Password: </Form.Label>
        <Form.Control
          type="password"
          placeholder="Confirm password"
          className="login-input"
        />
        <Button variant="primary" type="submit" className="login-submit">
          Sign Up
        </Button>
      </Form>
    </>
  );
};

export default SignUp;
