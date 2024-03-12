import React, { useState, useEffect } from "react";
import styles from "../../assets/scss/tabs.module.scss";
import axios from "axios";

// import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
// import { faComment, faHeart } from "@fortawesome/free-solid-svg-icons";

import {
  TabContent,
  TabPane,
  Container,
  Row,
  Col,
  Card,
  CardHeader,
  CardBody,
} from "reactstrap";
import { Link, NavLink } from "react-router-dom";

export default function Tabs({ categoryId }) {
  const [data, setData] = useState(null);
  const [filteredData, setFilteredData] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await axios.get("https://localhost:7028/api/PostApi");
        setData(response.data);
        setFilteredData(response.data); // Initially set filteredData to all posts
      } catch (error) {
        console.error("Error fetching data:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  useEffect(() => {
    // Update filteredData based on the selected categoryId
    if (categoryId) {
      const filtered = data.filter((post) => post.categoryId === categoryId);
      setFilteredData(filtered);
    } else {
      // If no categoryId is selected, show all posts
      setFilteredData(data);
    }
  }, [categoryId, data]);

  return (
    <div className="section section-tabs">
      <Container>
        <div className="title">
          <h3 className="mb-3">Recent Posts</h3>
        </div>

        <Row>
          {loading ? (
            <h1>Loading....</h1>
          ) : (
            <>
              {filteredData && filteredData.length > 0 ? (
                filteredData.map((post) => (
                  <Col key={post.postId} className="mr-auto" md="10" xl="5">
                    <div className="mb-3"></div>
                    <Card>
                      <CardHeader className={styles.image}>
                        <img src={require("assets/img/profile.jpg")} alt="" />
                      </CardHeader>
                      <CardBody>
                        <TabContent
                          className={`tab-space ${styles.text}`}
                          activeTab="link1"
                        >
                          <TabPane tabId="link1" className={styles.title}>
                            <div>
                              <a href="/details">
                                <h1>{post.title}</h1>
                              </a>
                              <h5>{post.content}</h5>
                            </div>

                            <p>{post.content}</p>

                            <div className={styles.more}>
                              <NavLink
                                tag={Link}
                                to={`/details/${post.postId}`}
                              >
                                Read more
                              </NavLink>
                            </div>
                          </TabPane>
                          <TabPane tabId="link2">
                            <p>
                              Completely synergize resource taxing relationships
                              via premier niche markets. Professionally
                              cultivate one-to-one customer service with robust
                              ideas. <br />
                              <br />
                              Dynamically innovate resource-leveling customer
                              service for state of the art customer service.
                            </p>
                          </TabPane>
                          <TabPane tabId="link3">
                            <p>
                              Efficiently unleash cross-media information
                              without cross-media value. Quickly maximize timely
                              deliverables for real-time schemas. <br />
                              <br />
                              Dramatically maintain clicks-and-mortar solutions
                              without functional solutions.
                            </p>
                          </TabPane>
                        </TabContent>
                      </CardBody>
                    </Card>
                  </Col>
                ))
              ) : (
                <p>No posts</p>
              )}
            </>
          )}
        </Row>
      </Container>
    </div>
  );
}
