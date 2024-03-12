import React, { useState } from "react";
import classnames from "classnames";
import axios from "axios";
import { useNavigate } from "react-router-dom";

import {
  Button,
  Card,
  CardHeader,
  CardBody,
  CardFooter,
  CardImg,
  CardTitle,
  Label,
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

const RegisterPage = () => {
  const [squares1to6, setSquares1to6] = useState("");
  const [squares7and8, setSquares7and8] = useState("");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");

  const [firstNameFocus, setFirstNameFocus] = useState(false);
  const [lastNameFocus, setLastNameFocus] = useState(false);
  const [emailFocus, setEmailFocus] = useState(false);
  const [passwordFocus, setPasswordFocus] = useState(false);
  const [confirmPasswordFocus, setConfirmPasswordFocus] = useState(false);

  const [loggedIn, setLoggedIn] = React.useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  const [errrorName, setErrrorName] = useState("");
  const [errorrLastName, setErrorrLastName] = useState("");
  const [errorEmail, setErrorEmail] = useState("");
  const [errorPassword, setErrorPassword] = useState("");

  const navigate = useNavigate();

  const registerUser = async () => {
    try {
      const response = await axios.post(
        "https://localhost:7028/api/AuthApi/register",
        {
          firstName,
          lastName,
          userName: email,
          password,
          confirmPassword,
        }
      );

      if (response.status === 200) {
        // Registration successful, redirect to login page
        navigate("/login");
      } else {
        // Handle other response statuses if needed
        console.error("Unexpected response:", response);
      }
    } catch (error) {
      setLoggedIn(true);
      setErrorMessage(error.response.data.errors.ConfirmPassword);
      setErrrorName(error.response.data.errors.FirstName);
      setErrorrLastName(error.response.data.errors.LastName);
      setErrorEmail(error.response.data.errors.UserName);
      setErrorPassword(error.response.data.errors.Password);

      console.error("Error registering user:", error);
    }
  };

  const handleRegister = (e) => {
    e.preventDefault();
    // Add validation logic if needed
    registerUser();
  };

  React.useEffect(() => {
    document.body.classList.toggle("register-page");
    document.documentElement.addEventListener("mousemove", followCursor);

    return function cleanup() {
      document.body.classList.toggle("register-page");
      document.documentElement.removeEventListener("mousemove", followCursor);
    };
  }, []);

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
                      <CardTitle tag="h4">Register</CardTitle>
                    </CardHeader>
                    <CardBody>
                      <Form className="form" onSubmit={handleRegister}>
                        <InputGroup
                          className={classnames({
                            "input-group-focus": firstNameFocus,
                          })}
                        >
                          <InputGroupAddon addonType="prepend">
                            <InputGroupText>
                              <i className="tim-icons icon-single-02" />
                            </InputGroupText>
                          </InputGroupAddon>
                          <Input
                            placeholder="First Name"
                            type="text"
                            onChange={(e) => setFirstName(e.target.value)}
                            onFocus={(e) => setFirstNameFocus(true)}
                            onBlur={(e) => setFirstNameFocus(false)}
                          />
                        </InputGroup>
                        {loggedIn ? <Label>{errrorName}</Label> : ""}
                        <InputGroup
                          className={classnames({
                            "input-group-focus": lastNameFocus,
                          })}
                        >
                          <InputGroupAddon addonType="prepend">
                            <InputGroupText>
                              <i className="tim-icons icon-single-02" />
                            </InputGroupText>
                          </InputGroupAddon>
                          <Input
                            placeholder="Last Name"
                            type="text"
                            onChange={(e) => setLastName(e.target.value)}
                            onFocus={(e) => setLastNameFocus(true)}
                            onBlur={(e) => setLastNameFocus(false)}
                          />
                        </InputGroup>
                        {loggedIn ? <Label>{errorrLastName}</Label> : ""}
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
                            placeholder="Email"
                            type="text"
                            onChange={(e) => setEmail(e.target.value)}
                            onFocus={(e) => setEmailFocus(true)}
                            onBlur={(e) => setEmailFocus(false)}
                          />
                        </InputGroup>
                        {loggedIn ? <Label>{errorEmail}</Label> : ""}
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
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            onFocus={(e) => setPasswordFocus(true)}
                            onBlur={(e) => setPasswordFocus(false)}
                          />
                        </InputGroup>
                        {loggedIn ? <Label>{errorPassword}</Label> : ""}
                        <InputGroup
                          className={classnames({
                            "input-group-focus": confirmPasswordFocus,
                          })}
                        >
                          <InputGroupAddon addonType="prepend">
                            <InputGroupText>
                              <i className="tim-icons icon-lock-circle" />
                            </InputGroupText>
                          </InputGroupAddon>
                          <Input
                            className={classnames({
                              "input-group-focus": confirmPasswordFocus,
                            })}
                            placeholder="Confirm Password"
                            type="password"
                            value={confirmPassword}
                            onChange={(e) => setConfirmPassword(e.target.value)}
                            onFocus={(e) => setConfirmPasswordFocus(true)}
                            onBlur={(e) => setConfirmPasswordFocus(false)}
                          />
                        </InputGroup>

                        {loggedIn ? <Label>{errorMessage}</Label> : ""}
                        <Button
                          className="btn-round"
                          color="primary"
                          size="lg"
                          type="submit"
                        >
                          Register
                        </Button>
                      </Form>
                    </CardBody>
                    <CardFooter></CardFooter>
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
};

export default RegisterPage;
