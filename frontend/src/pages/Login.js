import React, { useState } from "react";
import { Form, Button, Alert } from "react-bootstrap";
import { loginUser } from "../services/apiService";
import "./Login.css";

const Login = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);

  const handleLogin = async (e) => {
    e.preventDefault();
    setError(null);
    setSuccess(null);

    try {
      const response = await loginUser({ userName: username, password });
      
        localStorage.setItem("token", response.token); // Save token to local storage because only Fireox saves it in cookies

      setSuccess("Login successful!");
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <Form className="login-form" onSubmit={handleLogin}>
      {error && <Alert variant="danger">{error}</Alert>}
      {success && <Alert variant="success">{success}</Alert>}
      <Form.Label className="login-label">Username:</Form.Label>
      <Form.Control
        type="text"
        placeholder="Enter username"
        className="login-input"
        value={username}
        onChange={(e) => setUsername(e.target.value)}
        disabled={success}
      />
      <Form.Label className="login-label">Password: </Form.Label>
      <Form.Control
        type="password"
        placeholder="Enter password"
        className="login-input"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        disabled={success}
      />
      <Button
        variant="primary"
        type="submit"
        className="login-submit"
        disabled={success}
      >
        Log In
      </Button>
    </Form>
  );
};

export default Login;
