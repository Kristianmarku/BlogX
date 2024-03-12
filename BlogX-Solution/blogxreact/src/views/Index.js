import React from "react";

// core components
import IndexNavbar from "components/Navbars/IndexNavbar";
import PageHeader from "components/PageHeader/PageHeader.js";
import Footer from "components/Footer/Footer.js";
import Tabs from "./IndexSections/Tabs";
import Widget from "./IndexSections/Widget";

export default function Index() {
  const [selectedCategoryId, setSelectedCategoryId] = React.useState(null);

  const handleCategoryClick = (categoryId) => {
    setSelectedCategoryId(categoryId);
  };

  React.useEffect(() => {
    document.body.classList.toggle("index-page");

    return function cleanup() {
      document.body.classList.toggle("index-page");
    };
  }, []);

  return (
    <>
      <IndexNavbar />
      <Widget onCategoryClick={handleCategoryClick} />
      <div className="wrapper">
        <PageHeader />
        <div className="main">
          <Tabs categoryId={selectedCategoryId} />
        </div>
        <Footer />
      </div>
    </>
  );
}
