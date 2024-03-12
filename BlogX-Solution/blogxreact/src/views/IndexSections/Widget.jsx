import styles from "../../assets/scss/widget.module.scss";
import { useState, useEffect } from "react";
import axios from "axios";
import { Link } from "react-router-dom";
import { NavItem } from "reactstrap";

export default function Widget({ onCategoryClick }) {
  const [data, setData] = useState(null);
  const [latestPosts, setLatestPosts] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await axios.get(
          "https://localhost:7028/api/CategoryApi"
        );
        setData(response.data);
      } catch (error) {
        console.error("Error fetching categories:", error);
      }
    };

    fetchData();
  }, []);

  useEffect(() => {
    const fetchLatestPosts = async () => {
      try {
        const response = await axios.get(
          "https://localhost:7028/api/PostApi/GetLatestPosts"
        );
        setLatestPosts(response.data);
      } catch (error) {
        console.error("Error fetching latest posts:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchLatestPosts();
  }, []);

  return (
    <>
      {loading ? (
        <h1>Loading...</h1>
      ) : (
        <div className={styles.widget}>
          <div className={styles.wrapper}>
            <div className="">
              <h4>Categories</h4>
              <ul>
                {data &&
                  data.map((category) => (
                    <li
                      key={category.categoryId}
                      onClick={() => onCategoryClick(category.categoryId)}
                    >
                      {category.categoryName}
                    </li>
                  ))}
              </ul>
            </div>
            <div className="">
              <h4>Recent Posts</h4>
              <ul>
                {latestPosts &&
                  latestPosts.map((post) => (
                    <NavItem
                      tag={Link}
                      to={`/details/${post.postId}`}
                      key={post.postId}
                    >
                      <li>{post.content}</li>
                    </NavItem>
                  ))}
              </ul>
            </div>
          </div>
        </div>
      )}
    </>
  );
}
