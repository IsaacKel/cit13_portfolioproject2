import React, { useState, useContext, useEffect } from "react";
import { Form, Alert } from "react-bootstrap";
import { loginUser } from "../services/apiService";
import "./Login.css";
import { useNavigate } from "react-router-dom";
import AuthContext from "../components/AuthContext";

const Login = ({ onLoginSuccess, isFullPage }) => {
  const navigate = useNavigate();
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);
  const { login, isLoggedIn } = useContext(AuthContext);


    useEffect(() => {
        if (isFullPage && isLoggedIn) {
        navigate("/"); // Redirect to the home page if already logged in ( Only for full page loginsince users should'nt be redirected if they are viewing a title or name) 
    }
  }, [isLoggedIn, navigate]);


  const handleLogin = async (e) => {
    e.preventDefault();
    setError(null);
    setSuccess(null);

    try {
      const response = await loginUser({ userName: username, password });
      localStorage.setItem("token", response.token);

      login();

      setSuccess("Login successful!");
      if (onLoginSuccess) {
        onLoginSuccess();
      }
    } catch (err) {
      setError(err.message);
    }
  };

  const form = (
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
      <button type="submit" className="login-submit" disabled={success}>
        Log In
      </button>
    </Form>
  );

  return isFullPage ? (
    <div className="full-page-form-container">
      <div className="full-page-form">{form}</div>
    </div>
  ) : (
    form
  );
};

export default Login;

