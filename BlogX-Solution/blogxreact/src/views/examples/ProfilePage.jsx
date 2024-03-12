import React from "react";
import classnames from "classnames";
import axios from "axios";
import { Link } from "react-router-dom";

// javascript plugin used to create scrollbars on windows
import PerfectScrollbar from "perfect-scrollbar";
// reactstrap components
import {
  Button,
  Card,
  CardHeader,
  CardBody,
  Label,
  FormGroup,
  Input,
  FormText,
  NavItem,
  NavLink,
  Nav,
  Table,
  TabContent,
  TabPane,
  Container,
  Row,
  Col,
} from "reactstrap";

// core components
import IndexNavbar from "components/Navbars/IndexNavbar";
import Footer from "components/Footer/Footer.js";

let ps = null;

export default function ProfilePage() {
  const [data, setData] = React.useState(null);
  const [loading, setLoading] = React.useState(true);

  const handleDeletePost = async (postId) => {
    try {
      const postIndex = data.findIndex((post) => post.postId === postId);
      if (postIndex !== -1) {
        const newData = [...data];
        newData.splice(postIndex, 1);
        setData(newData);

        await axios.delete(`https://localhost:7028/api/PostApi/${postId}`);
        console.log(postId);

        fetchData();
      }
    } catch (error) {
      console.error("Error deleting post:", error);
    }
  };

  const fetchData = async () => {
    try {
      const cookieString = document.cookie;
      const userIdCookie = cookieString
        .split("; ")
        .find((cookie) => cookie.startsWith("userId="));

      let userId;
      if (userIdCookie) {
        userId = userIdCookie.split("=")[1];

        const response = await axios.get(
          `https://localhost:7028/api/PostApi/MyPosts/${userId}`
        );

        setData(response.data);
      }
    } catch (error) {
      console.error("Error fetching data:", error);
    } finally {
      setLoading(false);
    }
  };

  React.useEffect(() => {
    fetchData();
  }, []);

  const [latestPosts, setLatestPosts] = React.useState(null);

  React.useEffect(() => {
    const fetchLatestPosts = async () => {
      try {
        const response = await axios.get(
          "https://localhost:7028/api/PostApi/GetLatestPosts"
        );
        setLatestPosts(response.data);
        console.log(response);
      } catch (error) {
        console.error("Error fetching latest posts:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchLatestPosts();
  }, []);

  const [tabs, setTabs] = React.useState(1);
  React.useEffect(() => {
    if (navigator.platform.indexOf("Win") > -1) {
      document.documentElement.className += " perfect-scrollbar-on";
      document.documentElement.classList.remove("perfect-scrollbar-off");
      let tables = document.querySelectorAll(".table-responsive");
      for (let i = 0; i < tables.length; i++) {
        ps = new PerfectScrollbar(tables[i]);
      }
    }
    document.body.classList.toggle("profile-page");

    return function cleanup() {
      if (navigator.platform.indexOf("Win") > -1) {
        ps.destroy();
        document.documentElement.className += " perfect-scrollbar-off";
        document.documentElement.classList.remove("perfect-scrollbar-on");
      }
      document.body.classList.toggle("profile-page");
    };
  }, []);

  const cookieString = document.cookie;
  const userIdCookie = cookieString
    .split("; ")
    .find((cookie) => cookie.startsWith("user="));

  let userId;
  if (userIdCookie) {
    userId = userIdCookie.split("=")[1];
  }

  return (
    <>
      <IndexNavbar />
      <div className="wrapper">
        <div className="page-header">
          <img
            alt="..."
            className="dots"
            src={require("assets/img/dots.png")}
          />
          <img
            alt="..."
            className="path"
            src={require("assets/img/path4.png")}
          />
          <Container className="align-items-center">
            <Row>
              <Col className="ml-auto mr-auto" lg="4" md="6">
                <Card className="card-coin card-plain">
                  <CardHeader>
                    <img
                      alt="..."
                      className="img-center img-fluid rounded-circle"
                      src={require("assets/img/mike.jpg")}
                    />
                    <h4 className="title">{userId}</h4>
                  </CardHeader>
                  <CardBody>
                    <Nav
                      className="nav-tabs-primary justify-content-center"
                      tabs
                    >
                      <NavItem>
                        <NavLink
                          className={classnames({
                            active: tabs === 1,
                          })}
                          onClick={(e) => {
                            e.preventDefault();
                            setTabs(1);
                          }}
                          href="#pablo"
                        >
                          Posts
                        </NavLink>
                      </NavItem>

                      <NavItem>
                        <NavLink
                          className={classnames({
                            active: tabs === 3,
                          })}
                          onClick={(e) => {
                            e.preventDefault();
                            setTabs(3);
                          }}
                          href="#pablo"
                        >
                          Recent
                        </NavLink>
                      </NavItem>
                    </Nav>
                    <TabContent
                      className="tab-subcategories"
                      activeTab={"tab" + tabs}
                    >
                      <TabPane tabId="tab1">
                        <Table className="tablesorter" responsive>
                          <thead className="text-primary">
                            <tr>
                              <th className="header">Title</th>
                              <th className="header">id</th>
                              <th className="header">date</th>
                            </tr>
                          </thead>
                          <tbody>
                            {loading
                              ? "Loading..."
                              : data && data.length > 0
                              ? data.map((post) => (
                                  <tr>
                                    <td>{post.title}</td>
                                    <td>{post.postId}</td>
                                    <td>{post.dateCreated}</td>
                                  </tr>
                                ))
                              : ""}
                          </tbody>
                        </Table>
                      </TabPane>
                      <TabPane tabId="tab2">
                        <Row>
                          <Label sm="3">Pay to</Label>
                          <Col sm="9">
                            <FormGroup>
                              <Input
                                placeholder="e.g. 1Nasd92348hU984353hfid"
                                type="text"
                              />
                              <FormText color="default" tag="span">
                                Please enter a valid address.
                              </FormText>
                            </FormGroup>
                          </Col>
                        </Row>
                        <Row>
                          <Label sm="3">Amount</Label>
                          <Col sm="9">
                            <FormGroup>
                              <Input placeholder="1.587" type="text" />
                            </FormGroup>
                          </Col>
                        </Row>
                        <Button
                          className="btn-simple btn-icon btn-round float-right"
                          color="primary"
                          type="submit"
                        >
                          <i className="tim-icons icon-send" />
                        </Button>
                      </TabPane>
                      <TabPane tabId="tab3">
                        <Table className="tablesorter" responsive>
                          <thead className="text-primary">
                            <tr>
                              <th className="header">Titles</th>
                            </tr>
                          </thead>
                          <tbody>
                            {latestPosts &&
                              latestPosts.map((post) => (
                                <tr key={post.postId}>
                                  <td>{post.title}</td>
                                </tr>
                              ))}
                          </tbody>
                        </Table>
                      </TabPane>
                    </TabContent>
                  </CardBody>
                </Card>
              </Col>
            </Row>
          </Container>
        </div>
        <div className="section">
          <Container>
            {loading ? (
              <h1>Loading...</h1>
            ) : data && data.length > 0 ? (
              data.map((post) => (
                <Row key={post.postId} className="justify-content-between">
                  <Col className="mt-5" md="6">
                    <Row className="justify-content-between align-items-center">
                      <img src={require("assets/img/profile.jpg")} alt="" />
                    </Row>
                  </Col>
                  <Col className="mt-5" md="5">
                    <h1 className="">{post.title}</h1>

                    <p className="profile-description mt-1 text-left">
                      {post.content}
                    </p>
                    <div className="btn-wrapper pt-3 d-flex justify-content-between">
                      <div>
                        <NavItem>
                          <Button
                            className="btn-simple"
                            color="primary"
                            href="#pablo"
                            onClick={() => handleDeletePost(post.postId)}
                          >
                            <i className="tim-icons icon-book-bookmark" />{" "}
                            Delete
                          </Button>
                          <Button
                            className="btn-simple"
                            color="info"
                            tag={Link}
                            to={`/editPost/${post.postId}`}
                          >
                            <i className="tim-icons icon-bulb-63" /> Edit
                          </Button>
                        </NavItem>
                      </div>
                      <div></div>
                    </div>
                  </Col>
                </Row>
              ))
            ) : (
              <>
                <p>No posts create new one !</p>

                <Button
                  className="btn-simple mt-4"
                  color="info"
                  tag={Link}
                  to={`/newPost`}
                >
                  <i className="tim-icons icon-bulb-63" /> Create Post
                </Button>
              </>
            )}
          </Container>
        </div>

        <Footer />
      </div>
    </>
  );
}
