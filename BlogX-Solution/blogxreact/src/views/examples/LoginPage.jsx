import React from "react";
import { useState, useEffect } from "react";
import useAuth from "hooks/useAuth";
import classnames from "classnames";
import axios from "axios";
import { Link, useNavigate } from "react-router-dom";
import {
  Button,
  Card,
  CardHeader,
  CardBody,
  CardFooter,
  CardImg,
  CardTitle,
  Form,
  Input,
  InputGroupAddon,
  InputGroupText,
  InputGroup,
  Container,
  Row,
  Col,
} from "reactstrap";

import IndexNavbar from "components/Navbars/IndexNavbar";

import Footer from "components/Footer/Footer.js";

export default function RegisterPage() {
  const [squares1to6, setSquares1to6] = React.useState("");
  const [squares7and8, setSquares7and8] = React.useState("");

  const [emailFocus, setEmailFocus] = React.useState(false);
  const [passwordFocus, setPasswordFocus] = React.useState(false);

  const [loggedIn, setLoggedIn] = React.useState(false);

  const [userName, setUserName] = useState("");
  const [password, setPassword] = useState("");

  const [errorMessage, setErrorMessage] = useState("");

  const { setAuth, auth } = useAuth();

  const LOGIN_URL = "https://localhost:7028/api/AuthApi/login";

  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.post(LOGIN_URL, { userName, password });
      console.log(JSON.stringify(response?.data));
      const accessToken = response?.data?.token;
      document.cookie = `accessToken=${accessToken}; expires=${new Date(
        response?.data?.expiryDate
      ).toUTCString()}; path=/`;
      document.cookie = `userId=${response.data.user.id}; expires=${new Date(
        response?.data?.expiryDate
      ).toUTCString()}; path=/`;
      document.cookie = `user=${
        response.data.user.firstName
      }; expires=${new Date(response?.data?.expiryDate).toUTCString()}; path=/`;
      setAuth({ userName, password, accessToken });
      setLoggedIn(false);
      navigate("/home");
    } catch (err) {
      setErrorMessage(err.response.data.message);
      setLoggedIn(true);
    }
  };

  React.useEffect(() => {
    document.body.classList.toggle("register-page");
    document.documentElement.addEventListener("mousemove", followCursor);

    return function cleanup() {
      document.body.classList.toggle("register-page");
      document.documentElement.removeEventListener("mousemove", followCursor);
    };
  }, []);

  React.useEffect(() => {
    const isAuthenticated = document.cookie.includes("accessToken");

    if (isAuthenticated) {
      navigate("/home");
    }

    document.body.classList.toggle("register-page");
    document.documentElement.addEventListener("mousemove", followCursor);

    return function cleanup() {
      document.body.classList.toggle("register-page");
      document.documentElement.removeEventListener("mousemove", followCursor);
    };
  }, [navigate]);

  useEffect(() => {
    console.log(auth);
  }, [auth]);

  const followCursor = (event) => {
    let posX = event.clientX - window.innerWidth / 2;
    let posY = event.clientY - window.innerWidth / 6;
    setSquares1to6(
      "perspective(500px) rotateY(" +
        posX * 0.05 +
        "deg) rotateX(" +
        posY * -0.05 +
        "deg)"
    );
    setSquares7and8(
      "perspective(500px) rotateY(" +
        posX * 0.02 +
        "deg) rotateX(" +
        posY * -0.02 +
        "deg)"
    );
  };
  return (
    <>
      <IndexNavbar />

      <div className="wrapper">
        <div className="page-header">
          <div className="page-header-image" />
          <div className="content">
            <Container>
              <Row>
                <Col className="offset-lg-0 offset-md-3" lg="5" md="6">
                  <div
                    className="square square-7"
                    id="square7"
                    style={{ transform: squares7and8 }}
                  />
                  <div
                    className="square square-8"
                    id="square8"
                    style={{ transform: squares7and8 }}
                  />
                  <Card className="card-register">
                    <CardHeader>
                      <CardImg
                        alt="..."
                        src={require("assets/img/square-purple-1.png")}
                      />
                      <CardTitle tag="h4">Login</CardTitle>
                    </CardHeader>
                    <CardBody>
                      <Form className="form" onSubmit={handleSubmit}>
                        <InputGroup
                          className={classnames({
                            "input-group-focus": emailFocus,
                          })}
                        >
                          <InputGroupAddon addonType="prepend">
                            <InputGroupText>
                              <i className="tim-icons icon-email-85" />
                            </InputGroupText>
                          </InputGroupAddon>
                          <Input
                            placeholder="Name"
                            type="text"
                            onFocus={(e) => setEmailFocus(true)}
                            onBlur={(e) => setEmailFocus(false)}
                            onChange={(e) => setUserName(e.target.value)}
                          />
                        </InputGroup>
                        <InputGroup
                          className={classnames({
                            "input-group-focus": passwordFocus,
                          })}
                        >
                          <InputGroupAddon addonType="prepend">
                            <InputGroupText>
                              <i className="tim-icons icon-lock-circle" />
                            </InputGroupText>
                          </InputGroupAddon>
                          <Input
                            placeholder="Password"
                            type="text"
                            onFocus={(e) => setPasswordFocus(true)}
                            onBlur={(e) => setPasswordFocus(false)}
                            onChange={(e) => setPassword(e.target.value)}
                          />
                        </InputGroup>
                        {loggedIn ? (
                          <label style={{ color: "red" }}>{errorMessage}</label>
                        ) : (
                          ""
                        )}
                        <CardFooter>
                          <Button
                            className="btn-round"
                            color="primary"
                            size="lg"
                          >
                            Login
                          </Button>
                        </CardFooter>
                        <Button
                          className="btn-simple"
                          color="info"
                          tag={Link}
                          to={`/register`}
                        >
                          <i /> Register
                        </Button>
                      </Form>
                    </CardBody>
                  </Card>
                </Col>
              </Row>
              <div className="register-bg" />
              <div
                className="square square-1"
                id="square1"
                style={{ transform: squares1to6 }}
              />
              <div
                className="square square-2"
                id="square2"
                style={{ transform: squares1to6 }}
              />
              <div
                className="square square-3"
                id="square3"
                style={{ transform: squares1to6 }}
              />
              <div
                className="square square-4"
                id="square4"
                style={{ transform: squares1to6 }}
              />
              <div
                className="square square-5"
                id="square5"
                style={{ transform: squares1to6 }}
              />
              <div
                className="square square-6"
                id="square6"
                style={{ transform: squares1to6 }}
              />
            </Container>
          </div>
        </div>
        <Footer />
      </div>
    </>
  );
}
