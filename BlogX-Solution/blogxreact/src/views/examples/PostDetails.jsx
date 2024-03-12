import React, { useState } from "react";
import styles from "../../assets/scss/PostDetails.module.scss";
import { Container } from "reactstrap";
import ExamplesNavbar from "components/Navbars/IndexNavbar";
import Footer from "components/Footer/Footer.js";
import { useParams } from "react-router-dom";
import axios from "axios";

// let ps = null;
// ... (previous imports)

// ... (previous imports)

export default function PostDetails() {
  const [data, setData] = React.useState(null);
  const { id } = useParams();

  const [newComment, setNewComment] = useState("");
  const [submited, setSubmited] = useState(true);
  const [loading, setLoading] = useState(true);

  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await axios.get(
          `https://localhost:7028/api/PostApi/${id}`
        );
        setData(response.data);
      } catch (error) {
        console.error("Error fetching data:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [id, submited]);

  const handleSubmit = async (e) => {
    e.preventDefault();

    const POSTS_URL = `https://localhost:7028/api/CommentApi`;

    const cookieString = document.cookie;
    const userIdCookie = cookieString
      .split("; ")
      .find((cookie) => cookie.startsWith("userId="));

    let userId;
    if (userIdCookie) {
      userId = userIdCookie.split("=")[1];
    }

    const content = newComment;
    const postId = id;

    try {
      await axios.post(POSTS_URL, { postId, userId, content });

      setSubmited((prevSubmited) => !prevSubmited);
    } catch (err) {
      console.log(err);
    }
  };

  const handleEditComment = (commentId) => {
    // Add logic to handle editing a comment
    console.log(`Editing comment with ID ${commentId}`);
  };

  const handleDeleteComment = async (commentId) => {
    // Add logic to handle deleting a comment
    try {
      const response = await axios.get(
        `https://localhost:7028/api/CommentApi/${commentId}`
      );
      const commentUserId = response.data.userId;

      const cookieString = document.cookie;
      const currentUserIdCookie = cookieString
        .split("; ")
        .find((cookie) => cookie.startsWith("userId="));

      let currentUserId;
      if (currentUserIdCookie) {
        currentUserId = currentUserIdCookie.split("=")[1];
      }

      // Check if the current user is the owner of the comment
      if (currentUserId === commentUserId) {
        // If yes, proceed with deletion
        await axios.delete(
          `https://localhost:7028/api/CommentApi/${commentId}`
        );
        setSubmited((prevSubmited) => !prevSubmited);
      } else {
        // If not, show a message or handle as desired
        console.log("You can only delete your own comments.");
      }
    } catch (err) {
      console.log(err);
    }
  };

  const isCurrentUserComment = (commentUserId) => {
    const cookieString = document.cookie;
    const currentUserIdCookie = cookieString
      .split("; ")
      .find((cookie) => cookie.startsWith("userId="));

    let currentUserId;
    if (currentUserIdCookie) {
      currentUserId = currentUserIdCookie.split("=")[1];
    }

    return currentUserId === commentUserId;
  };

  return (
    <>
      <ExamplesNavbar />

      {loading ? (
        <h1>Loading...</h1>
      ) : (
        <>
          <Container className={styles.container}>
            <div className="container mt-5">
              <div className="card">
                <img
                  src={require("assets/img/denys.jpg")}
                  className="card-img-top img-fluid"
                  alt="Blog Post"
                />
                <div className="card-body">
                  <h2 className="card-title display-4">{data.title}</h2>
                  <p className="card-text lead">{data.content}</p>
                  <p className="card-text lead"></p>
                </div>
                <div className="card-footer text-muted">
                  {`Posted on ${data.dateCreated}`}
                </div>
              </div>
            </div>
            {/* Comment Section */}
            <div className="container mt-3">
              <div className="card">
                <div className="card-body">
                  <h3 className="card-title">Comments</h3>
                  {/* Comment Input */}
                  <div className="form-group">
                    <textarea
                      className="form-control"
                      placeholder="Add a comment..."
                      value={newComment}
                      onChange={(e) => setNewComment(e.target.value)}
                    ></textarea>
                  </div>
                  <button className="btn btn-primary" onClick={handleSubmit}>
                    Post Comment
                  </button>
                </div>
              </div>
            </div>
          </Container>

          {data.comments.map((comment) => (
            <Container key={comment.commentId}>
              <div className="card mt-3">
                <div className="card-body">
                  <h3 className="card-title">{comment.user.firstName}</h3>
                  <div>
                    <div className="mb-2">
                      <strong>{new Date().toLocaleString()}</strong>
                      <p>{comment.content}</p>
                      {/* Edit and Delete Buttons */}
                      {isCurrentUserComment(comment.user.userId) && (
                        <>
                          <button
                            className="btn btn-info mr-2"
                            onClick={() => handleEditComment(comment.commentId)}
                          >
                            Edit
                          </button>
                          <button
                            className="btn btn-danger"
                            onClick={() =>
                              handleDeleteComment(comment.commentId)
                            }
                          >
                            Delete
                          </button>
                        </>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            </Container>
          ))}
        </>
      )}

      <div className="wrapper">
        <Footer />
      </div>
    </>
  );
}
