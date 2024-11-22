// Login page using bootstrap

import React from "react";
import { Form, Button } from "react-bootstrap";
import "./Login.css";

const Login = () => {
  return (
    <>
      <Form className="login-form">
        <Form.Label className="login-label">Username:</Form.Label>
        <Form.Control
          type="username"
          placeholder="enter username.."
          className="login-input"
        />
        <Form.Label className="login-label">Password: </Form.Label>
        <Form.Control
          type="password"
          placeholder="enter password.."
          className="login-input"
        />
        <Button variant="primary" type="submit" className="login-submit">
          Log In
        </Button>
      </Form>
    </>
  );
};

export default Login;
