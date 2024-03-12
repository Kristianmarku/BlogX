import React, { useState, useEffect } from "react";
import {
  Container,
  Row,
  Col,
  Form,
  FormGroup,
  Label,
  Input,
  Button,
} from "reactstrap";
import axios from "axios";
import { useParams } from "react-router-dom";
import IndexNavbar from "components/Navbars/IndexNavbar";

const EditPostForm = () => {
  const { id } = useParams();

  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const [category, setCategory] = useState("");

  useEffect(() => {
    const fetchPostDetails = async () => {
      try {
        const response = await axios.get(
          `https://localhost:7028/api/PostApi/${id}`
        );

        const post = response.data;

        console.log(response);

        setTitle(post.title);
        setContent(post.content);
        setCategory(post.categoryId.toString());
      } catch (error) {
        console.error("Error fetching post details:", error);
      }
    };

    fetchPostDetails();
  }, [id]);

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      await axios.put(`https://localhost:7028/api/PostApi/${id}`, {
        postId: parseInt(id),
        title,
        content,
        categoryId: parseInt(category),
      });
    } catch (error) {
      console.error("Error updating post:", error);
    }
  };

  return (
    <>
      <IndexNavbar />
      <Container className="mt-5">
        <Row className="justify-content-md-center">
          <Col md="6">
            <h2 className="mb-4">Edit Post</h2>
            <Form onSubmit={handleSubmit}>
              <FormGroup>
                <Label for="formTitle">Title</Label>
                <Input
                  type="text"
                  placeholder="Enter post title"
                  value={title}
                  onChange={(e) => setTitle(e.target.value)}
                />
              </FormGroup>

              <FormGroup>
                <Label for="formContent">Content</Label>
                <Input
                  type="textarea"
                  rows="3"
                  placeholder="Enter post content"
                  value={content}
                  onChange={(e) => setContent(e.target.value)}
                />
              </FormGroup>

              <FormGroup>
                <Label for="formCategory">Category</Label>
                <Input
                  type="select"
                  value={category}
                  onChange={(e) => setCategory(e.target.value)}
                >
                  <option value="">Select a category</option>

                  <option value="1">Category 1</option>
                  <option value="2">Category 2</option>
                </Input>
              </FormGroup>

              <Button color="primary" type="submit">
                Update
              </Button>
            </Form>
          </Col>
        </Row>
      </Container>
    </>
  );
};

export default EditPostForm;
